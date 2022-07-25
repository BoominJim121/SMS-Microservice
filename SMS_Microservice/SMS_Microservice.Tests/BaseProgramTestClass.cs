using Flurl.Http;
using Flurl.Http.Testing;
using SMS_Microservice.Tests.TestDependancies;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Reflection;

namespace SMS_Microservice.Tests
{
    public class BaseProgramTestClass
    {
        public HttpTest HttpTest;
        public MemoryBusForTesting TestingBus;
        public MockEventPublisher mockPublisher; 

        public const string ThirdPartyServiceName = "http://thidpartysmsclient.com";

        public BaseProgramTestClass(Action entryPoint)
        {
            HttpTest = new();
            HttpTest.RespondWith(string.Empty, (int)HttpStatusCode.InternalServerError);
            ResetFlurlHttpTest();
            TestingBus = new MemoryBusForTesting();
            TestingBus.Init();
            mockPublisher = new MockEventPublisher(TestingBus);
            TestableMicroserviceConsumer.RegisterEventPublisher(mockPublisher);

            Task.Factory.StartNew(entryPoint);
            var breaker = new Stopwatch();
            breaker.Start();
            while (!TestingBus.BusStarted)
            {
                //wait
                if (breaker.Elapsed > TestingBus.MaxWaitTime)
                {
                    throw new TimeoutException();
                }
            }
            breaker.Stop();
        }

        private void ResetFlurlHttpTest()
        {
            var calls = typeof(HttpTest).GetField("_calls", BindingFlags.Instance | BindingFlags.NonPublic)!;
            calls.SetValue(HttpTest, new ConcurrentQueue<FlurlCall>());
            var setups = typeof(HttpTest).GetField("_filteredSetups", BindingFlags.Instance | BindingFlags.NonPublic)!;
            setups.SetValue(HttpTest, new List<FilteredHttpTestSetup>());
        }
    }
}
