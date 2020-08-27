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
    public class ModalDialog : ComponentBase
    {
        protected Modal Modal { get; set; }

        public virtual void Show() => Modal.Show();

        public virtual void Hide() => Modal.Hide();
    }
}
