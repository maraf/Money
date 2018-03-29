using Neptuo;
using Neptuo.Commands.Handlers;
using Neptuo.Models.Keys;
using Neptuo.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Commands.Handlers
{
    public class CategoryHandler : ICommandHandler<RenameCategory>, ICommandHandler<ChangeCategoryDescription>, ICommandHandler<ChangeCategoryColor>, ICommandHandler<ChangeCategoryIcon>, ICommandHandler<DeleteCategory>
    {
        private readonly IRepository<Category, IKey> repository;

        public CategoryHandler(IRepository<Category, IKey> repository)
        {
            Ensure.NotNull(repository, "repository");
            this.repository = repository;
        }

        public Task HandleAsync(RenameCategory command)
        {
            return Task.Factory.StartNew(() =>
            {
                Category category = repository.Get(command.CategoryKey);
                category.Rename(command.NewName);
                repository.Save(category);
            });
        }

        public Task HandleAsync(ChangeCategoryDescription command)
        {
            return Task.Factory.StartNew(() =>
            {
                Category category = repository.Get(command.CategoryKey);
                category.ChangeDescription(command.Description);
                repository.Save(category);
            });
        }

        public Task HandleAsync(ChangeCategoryColor command)
        {
            return Task.Factory.StartNew(() =>
            {
                Category category = repository.Get(command.CategoryKey);
                category.ChangeColor(command.Color);
                repository.Save(category);
            });
        }

        public Task HandleAsync(ChangeCategoryIcon command)
        {
            return Task.Factory.StartNew(() =>
            {
                Category category = repository.Get(command.CategoryKey);
                category.ChangeIcon(command.Icon);
                repository.Save(category);
            });
        }

        public Task HandleAsync(DeleteCategory command)
        {
            return Task.Factory.StartNew(() =>
            {
                Category category = repository.Get(command.CategoryKey);
                category.Delete();
                repository.Save(category);
            });
        }
    }
}
