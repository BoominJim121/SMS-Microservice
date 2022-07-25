using Platform.Messaging;

namespace SMS_Microservice
{
    public class EventPublisher : IEventPublisher
    {
        public Task Publish<T>(T toPublish) where T : class
        {
            // this is where the concrete implementation would go but is out side the scope of this test. 
            return Task.CompletedTask;
        }
    }
}
