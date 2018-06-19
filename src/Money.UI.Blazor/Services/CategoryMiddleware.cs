using Money.Models;
using Money.Models.Queries;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    internal class CategoryMiddleware : HttpQueryDispatcher.IMiddleware
    {
        private readonly List<CategoryModel> models = new List<CategoryModel>();

        public async Task<object> ExecuteAsync(object query, HttpQueryDispatcher.Next next)
        {
            if (query is ListAllCategory listAll)
            {
                if (models.Count == 0)
                {
                    models.Clear();
                    models.AddRange((List<CategoryModel>)await next(listAll));
                }

                return models.Select(c => c.Clone()).ToList();
            }
            else if (query is GetCategoryName categoryName)
            {
                CategoryModel model = Find(categoryName.CategoryKey);
                if (model != null)
                    return model.Name;
            }
            else if (query is GetCategoryIcon categoryIcon)
            {
                CategoryModel model = Find(categoryIcon.CategoryKey);
                if (model != null)
                    return model.Icon;
            }
            else if (query is GetCategoryColor categoryColor)
            {
                CategoryModel model = Find(categoryColor.CategoryKey);
                if (model != null)
                    return model.Color;
            }
            else if (query is GetCategoryNameDescription categoryNameDescription)
            {
                CategoryModel model = Find(categoryNameDescription.CategoryKey);
                if (model != null)
                    return new CategoryNameDescriptionModel(model.Name, model.Description);
            }

            return await next(query);
        }

        private CategoryModel Find(IKey key)
            => models.FirstOrDefault(c => c.Key.Equals(key));
    }
}
