using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace SMS_Microservice.Tests.TestDependancies
{
    public class TestMessageObserverBase
    {
        protected const string UnknownTestKey = "UnknownTest";

        protected static void AddMessageToStore(string key, Type type, string serializeObject,
            ConcurrentDictionary<string, List<(Type, string)>> messages)
        {
            messages.TryGetValue(key, out var container);

            if (container == null)
            {
                container = new List<(Type, string)>();
                messages.AddOrUpdate(
                    key,
                    x => container,
                    (x, y) => container = y);

                container.Add((type, serializeObject));
            }
            else
            {
                container.Add((type, serializeObject));
            }
        }

        protected static IEnumerable<T> GetMessages<T>(string testId,
            ConcurrentDictionary<string, List<(Type, string)>> messages)
        {
            messages.TryGetValue(testId, out var container);
            return container?
                       .Where(i => i.Item1 == typeof(T))
                       .Select(i => JsonConvert.DeserializeObject<T>(i.Item2))
                   ?? new List<T>();
        }
    }
}
