using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    public interface IAvailableMenuItemModel
    {
        string Identifier { get; }
        string Icon { get; }
        string Text { get; }
    }
}
