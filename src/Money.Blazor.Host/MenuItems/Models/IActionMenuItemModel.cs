using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    public interface IActionMenuItemModel
    {
        string Icon { get; }
        string Text { get; }
        string Url { get; }
        Type PageType { get; }
        Action OnClick { get; }
    }
}
