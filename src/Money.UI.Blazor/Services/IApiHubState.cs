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
        bool IsActive { get; }
        bool IsError { get; }

        event Action Changed;
    }
}
