using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services.Settings
{
    public interface IUserPreferenceService
    {
        bool TryLoad<T>(string containerPath, out T model) where T : class;
        bool TrySave<T>(string containerPath, T model);
    }
}
