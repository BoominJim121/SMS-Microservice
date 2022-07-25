using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendSMS.Client
{
    public interface IRetryPolicy
    {
        Task CreateAndExecutePolicy(string actionName, Func<int, Task> action);
    }
}
