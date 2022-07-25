using Flurl.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SendSMS.Client.Exceptions;

namespace SendSMS.Client
{
    public class SMSClient
    {
        private readonly IFlurlClient _flurlClient;
        private readonly ILogger<SMSClient> _logger;
        
        private readonly RetryPolicy _retryPolicy;

        public SMSClient(ILogger<SMSClient> logger, IFlurlClient flurlClient)
        {
            _logger = logger;
           
            _flurlClient = flurlClient;
            _retryPolicy = new RetryPolicy(_logger);
        }

        public async Task<bool> Send<T>(T toSend) where T : class
        {
            if (toSend == null) return false;
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(toSend));
                await _retryPolicy.CreateAndExecutePolicy(nameof(Send), async retryCount =>
                {
                    await _flurlClient.Request("/sendsms/")
                     .WithHeader("Content-Type", "application/json") //assumed headers
                     .PostAsync(content);
                });
                return true;
            }
            catch (FlurlHttpException ex) 
            {
                throw new SendSMSClientException(ex, "Error occurred when sending SMS to thirdparty sms client. see Flurl Exception for details.");
            }
        }
    }
}