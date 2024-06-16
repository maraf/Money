using Money.Components;
using Money.Models.Sorting;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Queries
{
    /// <summary>
    /// A query for getting a preferred color theme.
    /// </summary>
    public class GetThemeTypeProperty : IQuery<ThemeType>
    {
        public const string PropertyKey = "Theme";
    }
}
