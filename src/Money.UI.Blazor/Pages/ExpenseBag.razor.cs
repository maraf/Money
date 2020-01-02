using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Components;
using Money.Models.Loading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Pages
{
    public partial class ExpenseBag
    {
        protected List<CreateOutcome> Models { get; } = new List<CreateOutcome>();
        protected LoadingContext Loading { get; } = new LoadingContext();

        protected ModalDialog CreateModal { get; set; }

        protected void CreateNewExpense()
        {
            CreateModal.Show();
            StateHasChanged();
        }
    }
}
