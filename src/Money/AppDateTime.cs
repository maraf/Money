using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money;

public class AppDateTime
{
    public static DateTime Now => DateTime.UtcNow;
    public static DateTime Today => DateTime.UtcNow.Date;
}
