using MassTransit;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace SMS_Microservice.Tests.TestDependancies
{
    public class TestSendObserver : TestMessageObserverBase, ISendObserver
    {
        private static readonly ConcurrentDictionary<string, List<(Type, string)>> Messages = new ConcurrentDictionary<string, List<(Type, string)>>();
        public Task PreSend<T>(SendContext<T> context) where T : class
        {
            return Task.CompletedTask;
        }

        public Task PostSend<T>(SendContext<T> context) where T : class
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

        public Task SendFault<T>(SendContext<T> context, Exception exception) where T : class
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
