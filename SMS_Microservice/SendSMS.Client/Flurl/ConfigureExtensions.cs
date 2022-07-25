using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendSMS.Client.Flurl
{
    public static class ConfigureExtensions
    {
        public static FlurlConfiguration AddPlatformFlurl(this IServiceCollection serviceCollection)
        {

            serviceCollection.AddSingleton<IFlurlFactory, FlurlFactory>();
            return new FlurlConfiguration(serviceCollection);
        }
    }
}
