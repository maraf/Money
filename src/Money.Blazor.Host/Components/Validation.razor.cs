using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public partial class Validation
    {
        [Parameter]
        public List<string> ErrorMessages { get; set; }
    }
}
