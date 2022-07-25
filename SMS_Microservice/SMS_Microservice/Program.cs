using MassTransit;
using Platform.Messaging;
using Platform.Messaging.Events;
using SendSMS.Client;
using SendSMS.Client.Flurl;
using SMS_Microservice;
using SMS_Microservice.Consumers;

public class Program: TestableMicroserviceConsumer
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddPlatformFlurl()
            .AddClient<SMSClient>("thidpartysmsclient", "com");

            var eventPublisher = PublishEventBus != null ? PublishEventBus : new EventPublisher();
            services.AddSingleton<IEventPublisher>(eventPublisher);

            var mtConfig = MassTransitProviderSetup != null
                         ? (Action<IBusRegistrationConfigurator>)(configurator =>
                             MassTransitProviderSetup(hostContext, configurator, services))
                         : null;

            services.ConfigurePaltformMessaging(messagingConfig =>
            {
                messagingConfig.AddCommandConsumer<SendSMSCommandConsumer>();
                messagingConfig.AddEvent<ISmsSentEvent>();
            },
            mtConfig);

        });
}
