using MassTransit;
using Platform.Messaging;

namespace SMS_Microservice
{
    public class TestableMicroserviceConsumer
    {
        protected static Action<HostBuilderContext, IBusRegistrationConfigurator, IServiceCollection> MassTransitProviderSetup;
        protected static IEventPublisher PublishEventBus;
        protected TestableMicroserviceConsumer()
        {

        }

        public static void RegisterServiceBusProvider(Action<HostBuilderContext, IBusRegistrationConfigurator, IServiceCollection> massTransitProviderSetup)
        {
            MassTransitProviderSetup ??= massTransitProviderSetup;
        }

        public static void RegisterEventPublisher(IEventPublisher publisher)
        {
            PublishEventBus = publisher;
        }
    }
}
