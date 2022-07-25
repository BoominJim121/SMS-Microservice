using FluentAssertions;
using Flurl.Http.Testing;
using MassTransit;
using Platform.Messaging.Commands;
using Platform.Messaging.Events;
using SMS_Microservice.Tests.TestDependancies;
using System.Net;
using TestStack.BDDfy;
using TestStack.BDDfy.Xunit;
using Xunit;

namespace SMS_Microservice.Tests.ConsumerTests
{
    public class ProcessSMSSendCommandTests:BaseProgramTestClass
    {

       
        public ProcessSMSSendCommandTests()
            : base(() => Program.Main(System.Array.Empty<string>())){}

        [BddfyFact]
        public async Task SmsMessageSentAndEventPublishedWhenSMSSendMessageCommandIsRecived()
        {
            var testId = Guid.NewGuid().ToString();
            this.Given(_ => SetupTest(testId))
                .When(_ => _.PublishMessage(testId))
                .Then(_ => _.TheMessageShouldBeProcessed(testId))
                .And(_ => SendSMSToThirdPartyCalled(1))
                .And(_ => SMSSentEventIsPublished(testId))
                .BDDfy();
        }

        [BddfyFact]
        public async Task SmsMessageNotSentWhenSMSSendMessageCommandIsRecivedAndphoneNumberIsNull()
        {
            var testId = Guid.NewGuid().ToString();
            this.Given(_ => SetupTest(testId))
                .When(_ => _.PublishMessageWithNoPhoneNumber(testId))
                .Then(_ => _.TheMessageShouldThrowException(testId))
                .And(_ => SendSMSToThirdPartyNotCalled())
                .And(_ => SMSSentEventIsNotPublished(testId))
                .BDDfy();
        }

        [BddfyFact]
        public async Task SmsMessageNotSentWhenSMSSendMessageCommandIsRecivedAndSMSTextNull()
        {
            var testId = Guid.NewGuid().ToString();
            this.Given(_ => SetupTest(testId))
                .When(_ => _.PublishMessageWithNoSMSText(testId))
                .Then(_ => _.TheMessageShouldThrowException(testId))
                .And(_ => SendSMSToThirdPartyNotCalled())
                .And(_ => SMSSentEventIsNotPublished(testId))
                .BDDfy();
        }

        [BddfyFact]
        public async Task SmsMessageNotSentWhenSMSSendMessageCommandIsRecivedButSmsClientFailsTwice()
        {
            var testId = Guid.NewGuid().ToString();
            this.Given(_ => SetupTestReturnsInternalServerError(testId))
                .When(_ => _.PublishMessage(testId))
                .Then(_ => _.TheMessageShouldBeProcessed(testId))
                .And(_ => SendSMSToThirdPartyCalled(2))
                .And(_ => SMSSentEventIsNotPublished(testId))
                .BDDfy();
        }

        [BddfyFact]
        public async Task SmsMessageSentAndEventPublishedWhenSMSSendMessageCommandIsRecivedAndSMSClientOnlyFailsOnce()
        {
            var testId = Guid.NewGuid().ToString();
            this.Given(_ => SetupTestReturnsInternalServerErrorOnFirstAttemptSucces(testId))
                .When(_ => _.PublishMessage(testId))
                .Then(_ => _.TheMessageShouldBeProcessed(testId))
                .And(_ => SendSMSToThirdPartyCalled(2))
                .And(_ => SMSSentEventIsPublished(testId))
                .BDDfy();
        }

        protected void TheMessageShouldBeProcessed(string testId)
        {
            var (_, success, exception) = TestingBus.GetFullMessageResult(testId);

            success.Should().Be(true, "A fault occurred processing the message\n" + exception);
        }
        protected void TheMessageShouldThrowException(string testId)
        {
            var (_, success, exception) = TestingBus.GetFullMessageResult(testId);
            if (exception == null)
            {
                throw new Exception("A fault did not occur");
            }
        }

        public void SetupTest(string testId)
        {
            HttpTest.ForCallsTo($"{ThirdPartyServiceName}/sendsms/")
                .WithVerb(HttpMethod.Post)
                .RespondWith("success", 200);
            mockPublisher.RegisterTestId(testId);
        }

        public void SetupTestReturnsInternalServerError(string testId)
        {
            HttpTest.ForCallsTo($"{ThirdPartyServiceName}/sendsms/")
                .WithVerb(HttpMethod.Post)
                .RespondWith("Internal Server Error", 500);
            mockPublisher.RegisterTestId(testId);
        }

        public void SetupTestReturnsInternalServerErrorOnFirstAttemptSucces(string testId)
        {
            HttpTest.ForCallsTo($"{ThirdPartyServiceName}/sendsms/")
                .WithVerb(HttpMethod.Post)
                .RespondWith("Internal Server Error", 500)
                .RespondWith("Success", 200);
            mockPublisher.RegisterTestId(testId);
        }

        public void SendSMSToThirdPartyCalled(int numberOfCalls)
        {
            HttpTest
                .ShouldHaveCalled($"{ThirdPartyServiceName}/sendsms/")
                .WithVerb(HttpMethod.Post)
                .Times(numberOfCalls);
        }

        public void SendSMSToThirdPartyNotCalled()
        {
            HttpTest
                .ShouldNotHaveCalled($"{ThirdPartyServiceName}/sendsms/");
        }

        public void SMSSentEventIsPublished(string testId)
        {

            var events = TestingBus.GetPublishedMessages<SmsSent>(testId).ToList();
            events.Should().NotBeNull();
            events.Count().Should().Be(1);
        }

        public void SMSSentEventIsNotPublished(string testId)
        {

            var events = TestingBus.GetPublishedMessages<SmsSent>(testId).ToList();
            events.Should().BeEmpty();
        }

        public async Task PublishMessage(string messageId)
        {
            await TestingBus.PublishMessage<ISendSmsCommand>(new
            {
                phoneNumber = "+447795876334",
                SMSText = "paid $4"
            },
            messageId);
        }

        public async Task PublishMessageWithNoPhoneNumber(string messageId)
        {
            await TestingBus.PublishMessage<ISendSmsCommand>(new
            {
                SMSText = "paid $4"
            },
            messageId);
        }

        public async Task PublishMessageWithNoSMSText(string messageId)
        {
            await TestingBus.PublishMessage<ISendSmsCommand>(new
            {
                phoneNumber = "+447795876334"
            },
            messageId);
        }




    }
}
