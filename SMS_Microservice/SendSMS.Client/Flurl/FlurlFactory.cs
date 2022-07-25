
using Flurl.Http;
using Microsoft.Extensions.DependencyInjection;

namespace SendSMS.Client.Flurl
{
    public class FlurlFactory: IFlurlFactory
    {
        public T CreateFlurlClient<T>(IServiceProvider sp, string apiName, string appDomain)
        {
            var flurlClient = Create(apiName, appDomain);
            return ActivatorUtilities.CreateInstance<T>(sp, flurlClient);
        }

        private IFlurlClient Create(string apiName, string appDomain)
        {
            var flurlClient = new FlurlClient($"http://{apiName}.{appDomain}");
            return flurlClient;
        }
    }
}
