using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using MassTransit;
namespace SMS_Microservice.Tests.TestDependancies
{
    public class TestConsumeObserver : IConsumeObserver
    {
        private static readonly ConcurrentDictionary<string, (bool? success, Exception exception)> ProcessedMessages = new ConcurrentDictionary<string, (bool?, Exception)>();

        private static string TestId(Headers headers)
        {
            if (headers.TryGetHeader("TestId", out var testId))
            {
                return testId as string;
            }

            throw new ArgumentNullException(nameof(testId));
        }

        public static (bool completed, bool success, Exception exception) GetMessageResult(string testId)
        {
            var messageFound = ProcessedMessages.TryGetValue(testId, out var result);
            return (messageFound && result.success.HasValue, result.success ?? false, result.exception);
        }

        public static void ClearMessageResult(string testId)
        {
            ProcessedMessages.TryRemove(testId, out _);
        }

        public Task PreConsume<T>(ConsumeContext<T> context) where T : class
        {
            var testId = TestId(context.Headers);
            if (ProcessedMessages.TryAdd(testId, (null, null)))
            {
                return Task.CompletedTask;
            }

            throw new IndexOutOfRangeException($"no record of testid {testId}");
        }

        public Task PostConsume<T>(ConsumeContext<T> context) where T : class
        {
            var testId = TestId(context.Headers);
            if (!ProcessedMessages.ContainsKey(testId))
            {
                throw new IndexOutOfRangeException($"no record of testid {testId}");
            }

            ProcessedMessages[testId] = (true, null);
            return Task.CompletedTask;
        }

        public Task ConsumeFault<T>(ConsumeContext<T> context, System.Exception exception) where T : class
        {
            var testId = TestId(context.Headers);
            if (!ProcessedMessages.ContainsKey(testId))
            {
                throw new IndexOutOfRangeException($"no record of testid {testId}");
            }

            ProcessedMessages[testId] = (false, exception);
            return Task.CompletedTask;
        }
    }
}
