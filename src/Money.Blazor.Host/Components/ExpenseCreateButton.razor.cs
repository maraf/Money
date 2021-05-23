using Microsoft.AspNetCore.Components;
using Money.Services;
using Neptuo;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public partial class ExpenseCreateButton
    {
        [Inject]
        protected Navigator Navigator { get; set; }

        [Parameter]
        public IKey CategoryKey { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (CategoryKey == null)
                CategoryKey = KeyFactory.Empty(typeof(Category));
        }
    }
}
