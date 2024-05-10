using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components;

public interface IExpenseCreate
{
    void Show(IKey categoryKey);
    void Show(Price amount, string description, IKey categoryKey, bool isFixed);
}
