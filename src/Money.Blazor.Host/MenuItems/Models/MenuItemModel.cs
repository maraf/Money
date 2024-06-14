using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    public record MenuItemModel(
        string Identifier = null,
        string Icon = null,
        string Text = null,
        string Url = null,
        Type PageType = null,
        Action OnClick = null,
        bool IsBlurMenuAfterClick = false,
        bool IsRequired = false
    ) : IActionMenuItemModel, IAvailableMenuItemModel;
}
