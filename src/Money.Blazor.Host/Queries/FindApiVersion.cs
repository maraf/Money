using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Queries
{
    /// <summary>
    /// Vrátí aktuální verzi API (pokud již byla zjištěna).
    /// </summary>
    public class FindApiVersion : IQuery<Version>
    { }
}
