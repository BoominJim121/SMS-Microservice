using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendSMS.Client.Flurl
{
    public  class FlurlConfiguration
    {
        private readonly IServiceCollection _serviceCollection;

        public FlurlConfiguration(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        public FlurlConfiguration AddClient<T>(string apiName, string apiDomain)
            where T : class
        {
            _serviceCollection.AddScoped(sp => sp.GetRequiredService<IFlurlFactory>().CreateFlurlClient<T>(sp, apiName, apiDomain));
            return this;
        }
    }
}
