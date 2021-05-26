using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    public class MenuItemModel : IActionMenuItemModel, IAvailableMenuItemModel
    {
        public string Identifier { get; set; }
        public string Icon { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }
        public Type PageType { get; set; }
        public Action OnClick { get; set; }
    }
}
