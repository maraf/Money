using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neptuo.Models.Keys;
using Windows.UI;

namespace Money.Services.Models
{
    public class CategoryWithAmountModel : CategoryModel
    {
        public Price TotalAmount { get; private set; }

        public CategoryWithAmountModel(IKey key, string name, string description, Color color, string icon, Price totalAmount) 
            : base(key, name, description, color, icon)
        {
            TotalAmount = totalAmount;
        }
    }
}
