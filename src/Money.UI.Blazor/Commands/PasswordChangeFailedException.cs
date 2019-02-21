using Neptuo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Commands
{
    /// <summary>
    /// An exception raised when user password change fails.
    /// </summary>
    public class PasswordChangeFailedException : AggregateRootException
    { }
}
