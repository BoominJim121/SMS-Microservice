using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendSMS.Client.Exceptions
{
    public class SendSMSClientException: Exception
    {
        FlurlHttpException FlurlException { get; }
        public SendSMSClientException(FlurlHttpException ex, string message)
            : base(message)
        {
            FlurlException = ex;
        }
    }
}
