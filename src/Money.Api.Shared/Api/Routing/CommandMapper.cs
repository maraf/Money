using Money.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Api.Routing
{
    public class CommandMapper : TypeMapper
    {
        public CommandMapper()
        {
            Add<CreateOutcome>("outcome-create");
            Add<ChangeOutcomeAmount>("outcome-change-amount");
            Add<ChangeOutcomeDescription>("outcome-change-description");
            Add<ChangeOutcomeWhen>("outcome-change-when");
            Add<DeleteOutcome>("outcome-delete");

            Add<CreateExpenseTemplate>("expense-template-create");
            Add<ChangeExpenseTemplateAmount>("expense-template-change-amount");
            Add<ChangeExpenseTemplateDescription>("expense-template-change-description");
            Add<ChangeExpenseTemplateCategory>("expense-template-change-category");
            Add<ChangeExpenseTemplateFixed>("expense-template-change-fixed");
            Add<SetExpenseTemplateMonthlyRecurrence>("expense-template-set-recurrence-monthly");
            Add<SetExpenseTemplateSingleRecurrence>("expense-template-set-recurrence-single");
            Add<ClearExpenseTemplateRecurrence>("expense-template-clear-recurrence");
            Add<DeleteExpenseTemplate>("expense-template-delete");

            Add<CreateIncome>("income-create");
            Add<ChangeIncomeAmount>("income-change-amount");
            Add<ChangeIncomeDescription>("income-change-description");
            Add<ChangeIncomeWhen>("income-change-when");
            Add<DeleteIncome>("income-delete");

            Add<CreateCategory>("category-create");
            Add<ChangeCategoryColor>("category-change-color");
            Add<ChangeCategoryIcon>("category-change-icon");
            Add<ChangeCategoryDescription>("category-change-description");
            Add<RenameCategory>("category-rename");
            Add<DeleteCategory>("category-delete");

            Add<CreateCurrency>("currency-create");
            Add<ChangeCurrencySymbol>("currency-change-symbol");
            Add<SetCurrencyAsDefault>("currency-set-as-default");
            Add<SetExchangeRate>("currency-exchangerate-set");
            Add<RemoveExchangeRate>("currency-exchangerate-remove");
            Add<DeleteCurrency>("currency-delete");

            Add<ChangePassword>("user-change-password");
            Add<ChangeEmail>("user-change-email");
            Add<SetUserProperty>("user-set-property");
        }
    }
}
