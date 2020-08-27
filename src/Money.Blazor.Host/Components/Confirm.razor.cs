using Microsoft.AspNetCore.Components;
using Money.Components.Bootstrap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public partial class Confirm
    {
        [Parameter]
        public string Message { get; set; }

        [Parameter]
        public Action OnConfirmed { get; set; }

        protected void OnPrimaryButtonClick()
        {
            Modal.Hide();
            OnConfirmed?.Invoke();
        }
    }
}
