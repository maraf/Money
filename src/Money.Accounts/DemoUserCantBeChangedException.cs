using Neptuo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money
{
    /// <summary>
    /// An excception raised when demo user tries to change it's account.
    /// </summary>
    public class DemoUserCantBeChangedException : AggregateRootException
    { }
}
