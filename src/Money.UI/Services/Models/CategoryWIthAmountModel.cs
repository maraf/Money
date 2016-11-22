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

        public CategoryWithAmountModel(IKey key, string name, Color color, Price totalAmount) 
            : base(key, name, color)
        {
            TotalAmount = totalAmount;
        }
    }
}
