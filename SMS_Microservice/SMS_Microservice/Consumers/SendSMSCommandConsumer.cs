using MassTransit;
using Platform.Messaging;
using Platform.Messaging.Commands;
using Platform.Messaging.Events;
using SendSMS.Client;
using SendSMS.Client.Exceptions;
using SendSMS.Client.Requests;
using SMS_Microservice.Exceptions;

namespace SMS_Microservice.Consumers
{
    public class SendSMSCommandConsumer : IConsumer<ISendSmsCommand>
    {
        private readonly SMSClient _smsSendClient;
        private readonly ILogger<SendSMSCommandConsumer> _logger;
        private readonly IEventPublisher _publishser;

        public SendSMSCommandConsumer(SMSClient smsSendClient, ILogger<SendSMSCommandConsumer> logger, IEventPublisher publishser)
        {
            _smsSendClient = smsSendClient;
            _logger = logger;
            _publishser = publishser;
        }

        public async Task Consume(ConsumeContext<ISendSmsCommand> context)
        {
            var phoneNumber = context.Message.PhoneNumber;
            var smsMessage = context.Message.SMSText;
            if(phoneNumber == null || smsMessage == null)
            {
                _logger.LogError("Command recieved Command recieved without phone number or sms message sms can not be sent. {command} ", context.Message);
                throw new InvalidSMSCommandException($"Command recieved without phone number or sms message sms can not be sent.");
            }
            _logger.LogInformation("Command Recieved for SMS to {number}, processing.", phoneNumber);

            try
            {
                var success = await _smsSendClient.Send(new SmsMessage(phoneNumber, smsMessage));
                if (success)
                {
                    await _publishser.Publish<SmsSent>(new SmsSent(phoneNumber, smsMessage, DateTimeOffset.UtcNow));
                }
            }
            catch (SendSMSClientException ex)
            {
                _logger.LogError(ex, "Message to {phoneNumber} failed to send due to error from HttpClient retry once already attempted do not retry again.", phoneNumber);
                //bury exception as we don't want the command sender to retry based on exception handling. 
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Message to {phoneNumber} failed to send due to error NOT from HttpClient retry", phoneNumber);
                throw;
            }

        }
    }
}
