using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public enum PagingLoadStatus
    {
        /// <summary>
        /// When there are more items available.
        /// </summary>
        HasNextPage,

        /// <summary>
        /// When this page is the last one.
        /// </summary>
        LastPage,

        /// <summary>
        /// When loading current page resulted in zero items.
        /// </summary>
        EmptyPage
    }
}
