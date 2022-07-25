using MassTransit;
namespace SMS_Microservice.Tests.TestDependancies
{
    public class TestBusObserver : IBusObserver
    {
        public bool Started { get; private set; }

        public Task PostCreate(IBus bus)
        {
            return Task.CompletedTask;
        }

        public Task CreateFaulted(System.Exception exception)
        {
            throw exception;
        }

        public Task PreStart(IBus bus)
        {
            return Task.CompletedTask;
        }

        public Task PostStart(IBus bus, Task<BusReady> busReady)
        {
            Started = true;
            return Task.CompletedTask;
        }

        public Task StartFaulted(IBus bus, System.Exception exception)
        {
            throw exception;
        }

        public Task PreStop(IBus bus)
        {
            return Task.CompletedTask;
        }

        public Task PostStop(IBus bus)
        {
            return Task.CompletedTask;
        }

        public Task StopFaulted(IBus bus, System.Exception exception)
        {
            return Task.CompletedTask;
        }

        void IBusObserver.PostCreate(IBus bus)
        {
        }

        void IBusObserver.CreateFaulted(Exception exception)
        {
            throw exception;
        }
    }
}
