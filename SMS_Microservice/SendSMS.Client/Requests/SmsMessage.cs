using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendSMS.Client.Requests
{
    public record SmsMessage (string phoneNumber, string message)
    {
    }
}
