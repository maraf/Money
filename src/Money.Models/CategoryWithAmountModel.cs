using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neptuo.Formatters.Metadata;
using Neptuo.Models.Keys;

namespace Money.Models
{
    public class CategoryWithAmountModel : CategoryModel
    {
        [CompositeProperty(1, Version = 1)]
        [CompositeProperty(1, Version = 2)]
        public override IKey Key => base.Key;

        [CompositeProperty(2, Version = 1)]
        [CompositeProperty(2, Version = 2)]
        public override string Name => base.Name;

        [CompositeProperty(3, Version = 1)]
        [CompositeProperty(3, Version = 2)]
        public override string Description => base.Description;

        [CompositeProperty(4, Version = 1)]
        [CompositeProperty(4, Version = 2)]
        public override Color Color => base.Color;

        [CompositeProperty(5, Version = 1)]
        [CompositeProperty(5, Version = 2)]
        public override string Icon => base.Icon;

        [CompositeProperty(6, Version = 1)]
        public Price TotalAmount { get; private set; }

        [CompositeProperty(6, Version = 2)]
        public Price NonFixedAmount { get; private set; }

        [CompositeProperty(7, Version = 2)]
        public Price FixedAmount { get; private set; }

        [CompositeVersion]
        public override int Version { get => base.Version; set => base.Version = value; }

        [CompositeConstructor(Version = 1)]
        public CategoryWithAmountModel(IKey key, string name, string description, Color color, string icon, Price totalAmount) 
            : base(key, name, description, color, icon)
        {
            TotalAmount = totalAmount;
        }

        [CompositeConstructor(Version = 2)]
        public CategoryWithAmountModel(IKey key, string name, string description, Color color, string icon, Price nonFixedAmount, Price fixedAmount) 
            : this(key, name, description, color, icon, nonFixedAmount + fixedAmount)
        {
            NonFixedAmount = nonFixedAmount;
            FixedAmount = fixedAmount;

            Version = 2;
        }
    }
}
