using Flurl.Http;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SendSMS.Client
{
    public class RetryPolicy : IRetryPolicy
    {
        private readonly ILogger _logger;
        private int _retryCount;
        public RetryPolicy(ILogger logger)
        {
            _logger = logger;
            _retryCount = 0;
        }
        public async Task CreateAndExecutePolicy(string actionName, Func<int, Task> action)
        {
            _retryCount = 0;

            var policy = Polly.Policy
                .Handle<FlurlHttpException>(
                    exception => exception.StatusCode != (int)HttpStatusCode.BadRequest
                    && exception.StatusCode != (int)HttpStatusCode.NotFound
                    && exception.StatusCode != (int)HttpStatusCode.Conflict
                    && exception.StatusCode != (int)HttpStatusCode.UnprocessableEntity)
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromMilliseconds(100)//only retyr once as multiple messages could be a problem. 
                }, (exception, timeSpan, retryCount, context) =>
                {
                    if (!(exception is FlurlHttpException)) return;

                    var boominRequestException = (FlurlHttpException)exception;
                    _logger.LogWarning("Failed to {ActionName} on retry {RetryCount} due to {FlurlHttpException} {ExceptionStatusCode} - {ExceptionMessage}. Attempting again.",
                        actionName,
                        retryCount,
                        nameof(FlurlHttpException),
                        boominRequestException.StatusCode,
                        boominRequestException.Message);

                    _retryCount = retryCount;
                });

            var result = await policy.ExecuteAndCaptureAsync(() => action(_retryCount));

            if (result.Outcome == OutcomeType.Successful)
            {
                return;
            }

            throw result.FinalException;
        }
    }
}
