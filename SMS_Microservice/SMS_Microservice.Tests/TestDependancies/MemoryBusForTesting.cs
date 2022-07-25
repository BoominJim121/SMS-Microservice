using Flurl.Http.Testing;
using MassTransit;
using SMS_Microservice.Consumers;
using System.Diagnostics;
using System.Reflection;

namespace SMS_Microservice.Tests.TestDependancies
{
    public class MemoryBusForTesting
    {
        private readonly TimeSpan _maxWaitTime = new TimeSpan(0, 0, 1, 0);

        public readonly TimeSpan MaxWaitTime = new TimeSpan(0, 0, 30, 0);
        public static IBusControl MemoryBus { get; private set; }
        public bool BusStarted => BusObserver.Started;

        private static readonly TestBusObserver BusObserver = new TestBusObserver();
        private static readonly TestConsumeObserver ConsumeObserver = new TestConsumeObserver();
        private static readonly TestPublishObserver PublishObserver = new TestPublishObserver();
        private static readonly TestSendObserver SendObserver = new TestSendObserver();

        private static void ConfigureMassTransitProvider(HostBuilderContext hostContext, IBusRegistrationConfigurator mt, IServiceCollection services)
        {
            mt.AddConsumers(Assembly.GetCallingAssembly());

            var serviceProvider = DependencyInjectionOverrides(services);

            MemoryBus = Bus.Factory.CreateUsingInMemory(x =>
            {
                x.ClearSerialization();
                x.UseNewtonsoftJsonSerializer();
                x.UseNewtonsoftJsonDeserializer();

                x.ConnectBusObserver(BusObserver);
                x.ReceiveEndpoint("TestQueue", configurator =>
                {
                    configurator.Consumer<SendSMSCommandConsumer>(serviceProvider);
                });
            });

            MemoryBus.ConnectConsumeObserver(ConsumeObserver);
            MemoryBus.ConnectPublishObserver(PublishObserver);
            MemoryBus.ConnectSendObserver(SendObserver);

            MemoryBus.Start();
            mt.AddBus(x => MemoryBus);
        }

        private static ServiceProvider DependencyInjectionOverrides(IServiceCollection services)
        {
            services.AddSingleton<HttpTest>(sp => new HttpTest());
            return services.BuildServiceProvider();
        }

        protected bool GetMessageResult(string testId)
        {
            var breaker = new Stopwatch();
            breaker.Start();
            while (!TestConsumeObserver.GetMessageResult(testId).completed)
            {
                if (breaker.Elapsed > _maxWaitTime)
                {
                    throw new TimeoutException();
                }
            }
            breaker.Stop();

            return TestConsumeObserver.GetMessageResult(testId).success;
        }

        public (bool completed, bool success, Exception exception) GetFullMessageResult(string testId)
        {
            var breaker = new Stopwatch();
            breaker.Start();
            while (!TestConsumeObserver.GetMessageResult(testId).completed)
            {
                if (breaker.Elapsed > _maxWaitTime)
                {
                    throw new TimeoutException("The event was not processed within the timeout, check the consumer is present and that you are publishing the event");
                }
            }
            breaker.Stop();

            return TestConsumeObserver.GetMessageResult(testId);
        }

        public IEnumerable<T> GetPublishedMessages<T>(string testId) where T : class
        {
            return TestPublishObserver.GetMessages<T>(testId);
        }

        public IEnumerable<T> GetSentMessages<T>(string testId) where T : class
        {
            return TestSendObserver.GetMessages<T>(testId);
        }
       
        public async Task PublishMessage<T>(object message, string uniqueTestId)
            where T : class
        {
            await MemoryBus.Publish<T>(message, context =>
            {
                context.Headers.Set("TestId", uniqueTestId);
            });
        }

        public void Init()
        {
            Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");
            TestableMicroserviceConsumer.RegisterServiceBusProvider(ConfigureMassTransitProvider);
        }
    }
}
