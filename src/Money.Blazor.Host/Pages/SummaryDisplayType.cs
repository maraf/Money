using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Pages;

public enum SummaryDisplayType
{
    [Description("Total")]
    Total,
    [Description("Diff")]
    Diff,
    [Description("Balance")]
    Balance
}
