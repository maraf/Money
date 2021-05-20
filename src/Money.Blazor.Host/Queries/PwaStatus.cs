using Neptuo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Queries
{
    public class PwaStatus
    {
        public bool IsInstallable { get; }
        public bool IsUpdateable { get; }

        public PwaStatus(bool isInstallable, bool isUpdateable)
        {
            IsInstallable = isInstallable;
            IsUpdateable = isUpdateable;
        }
    }
}
