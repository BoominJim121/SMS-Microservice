using Platform.Messaging;
using System.Collections.Concurrent;

namespace SMS_Microservice.Tests.TestDependancies
{
    public class MockEventPublisher: IEventPublisher
    {
        public MemoryBusForTesting _testingBus;
        private string _testId;

        public MockEventPublisher(MemoryBusForTesting testingBus)
        {
            _testingBus = testingBus;
        }

        public void RegisterTestId(string testId)
        {
            _testId = testId;
        }
        public async Task Publish<T>(T toPublish) where T : class
        {
            await _testingBus.PublishMessage<T>(toPublish, _testId);
        }
    }
}
