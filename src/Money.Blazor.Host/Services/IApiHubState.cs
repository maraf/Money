using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    public interface IApiHubState
    {
        ApiHubStatus Status { get; }

        event Action<ApiHubStatus, Exception> Changed;
    }
}
