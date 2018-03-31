using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neptuo.Models.Keys;

namespace Money.Models
{
    public class CategoryWithAmountModel : CategoryModel
    {
        public override IKey Key => base.Key;
        public override string Name => base.Name;
        public override string Description => base.Description;
        public override Color Color => base.Color;
        public override string Icon => base.Icon;

        public Price TotalAmount { get; private set; }

        public CategoryWithAmountModel(IKey key, string name, string description, Color color, string icon, Price totalAmount) 
            : base(key, name, description, color, icon)
        {
            TotalAmount = totalAmount;
        }
    }
}
