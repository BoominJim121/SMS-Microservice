using System.Collections.Concurrent;
using MassTransit;
using Newtonsoft.Json;
namespace SMS_Microservice.Tests.TestDependancies
{
    public class TestPublishObserver: TestMessageObserverBase, IPublishObserver
    {
        private static readonly ConcurrentDictionary<string, List<(Type, string)>> Messages = new ConcurrentDictionary<string, List<(Type, string)>>();
        public Task PrePublish<T>(PublishContext<T> context) where T : class
        {
            return Task.CompletedTask;
        }

        public Task PostPublish<T>(PublishContext<T> context) where T : class
        {
            var serializeObject = JsonConvert.SerializeObject(context.Message);
            if (context.Headers.TryGetHeader("TestId", out var testId))
            {
                var testIdStr = testId as string;
                AddMessageToStore(testIdStr, typeof(T), serializeObject, Messages);
            }
            else
            {
                AddMessageToStore(UnknownTestKey, typeof(T), serializeObject, Messages);
            }

            return Task.CompletedTask;
        }

        public Task PublishFault<T>(PublishContext<T> context, Exception exception) where T : class
        {
            return Task.CompletedTask;
        }

        public static IEnumerable<T> GetMessages<T>(string testId = UnknownTestKey)
        {
            return GetMessages<T>(testId, Messages);
        }

        public static void ClearMessageResult(string testId)
        {
            Messages.TryRemove(testId, out _);
        }
    }
}
