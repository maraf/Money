using Money.Events;
using Money.Services.Models.Queries;
using Neptuo;
using Neptuo.Events.Handlers;
using Neptuo.Models.Keys;
using Neptuo.Queries.Handlers;
using Neptuo.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;

namespace Money.Services.Models.Builders
{
    /// <summary>
    /// A builder for querying categories.
    /// </summary>
    public class CategoryBuilder : IEventHandler<CategoryCreated>, IQueryHandler<ListAllCategory, IEnumerable<CategoryModel>>
    {
        private readonly ApplicationDataContainer container;

        public CategoryBuilder(ApplicationDataContainer container)
        {
            Ensure.NotNull(container, "container");
            this.container = container.CreateContainer("Categories", ApplicationDataCreateDisposition.Always);
        }

        public Task<IEnumerable<CategoryModel>> HandleAsync(ListAllCategory query)
        {
            List<CategoryModel> result = new List<CategoryModel>();
            foreach (ApplicationDataContainer categoryContainer in container.Containers.Values)
            {
                IKey categoryKey = GuidKey.Create(
                    Guid.Parse(categoryContainer.Name),
                    KeyFactory.Empty(typeof(Category)).Type
                );

                string rawColor = (string)categoryContainer.Values["Color"];
                byte[] colorBytes = rawColor.Split(';').Select(c => Byte.Parse(c)).ToArray();

                result.Add(new CategoryModel(
                    categoryKey, 
                    (string)categoryContainer.Values["Name"],
                    Color.FromArgb(colorBytes[0], colorBytes[1], colorBytes[2], colorBytes[3])
                ));
            }

            return Task.FromResult<IEnumerable<CategoryModel>>(result);
        }

        public Task HandleAsync(CategoryCreated payload)
        {
            ApplicationDataContainer categoryContainer = container
                .CreateContainer(payload.AggregateKey.AsGuidKey().Guid.ToString(), ApplicationDataCreateDisposition.Always);

            categoryContainer.Values["Name"] = payload.Name;
            categoryContainer.Values["Color"] = String.Concat(payload.Color.A, ";", payload.Color.R, ";", payload.Color.G, ";", payload.Color.B);
            return Async.CompletedTask;
        }
    }
}
