

namespace SendSMS.Client.Flurl
{
    public interface IFlurlFactory
    {
        T CreateFlurlClient<T>(IServiceProvider sp, string apiName, string appDomain);
    }
}
