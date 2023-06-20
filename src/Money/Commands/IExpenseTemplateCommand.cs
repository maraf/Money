using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Commands;

public interface IExpenseTemplateCommand
{
    IKey ExpenseTemplateKey { get; }
}
