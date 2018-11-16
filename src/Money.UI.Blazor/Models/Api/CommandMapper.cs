using Money.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models.Api
{
    public class CommandMapper : TypeMapper
    {
        public CommandMapper()
        {
            Add<CreateOutcome>("outcome-create");
            Add<ChangeOutcomeAmount>("outcome-change-amount");
            Add<ChangeOutcomeDescription>("outcome-change-description");
            Add<ChangeOutcomeWhen>("outcome-change-when");
            Add<DeleteOutcome>("outcome-change-delete");
        }
    }
}
