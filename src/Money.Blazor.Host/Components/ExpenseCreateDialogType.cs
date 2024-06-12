using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components;

public enum ExpenseCreateDialogType
{
    [Description("Standard")]
    Standard,
    [Description("Wizard")]
    Wizard
}
