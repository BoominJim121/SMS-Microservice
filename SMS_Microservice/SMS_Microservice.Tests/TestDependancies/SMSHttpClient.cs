using Flurl.Http.Testing;

namespace SMS_Microservice.Tests.TestDependancies
{
    public class SMSHttpClientTestClient
    {
        private const string ServiceName = "https://thirdaprtySMSClient.com";
        private readonly HttpTest _httpTest;


        public void AddPlatformTestFlurl(IServiceCollection services)
        {
           
        }

        public void SendSMSReturns(string responseText, int responseCode)
        {
            _httpTest
                .ForCallsTo($"{ServiceName}/sendsms")
                .WithVerb(HttpMethod.Post)
                .RespondWith(responseText, responseCode);
        }
    }
}
