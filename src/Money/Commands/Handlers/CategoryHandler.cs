using Neptuo.Activators;
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
    public class CategoryHandler : AggregateRootCommandHandler<Category>, 
        ICommandHandler<CreateCategory>, 
        ICommandHandler<RenameCategory>, 
        ICommandHandler<ChangeCategoryDescription>, 
        ICommandHandler<ChangeCategoryColor>, 
        ICommandHandler<ChangeCategoryIcon>, 
        ICommandHandler<DeleteCategory>
    {
        public CategoryHandler(IFactory<IRepository<Category, IKey>> repositoryFactory) 
            : base(repositoryFactory)
        { }

        public Task HandleAsync(CreateCategory command) => WithCommand(command.Key).Execute(() => new Category(command.Name, command.Description, command.Color));
        public Task HandleAsync(RenameCategory command) => WithCommand(command.Key).Execute(command.CategoryKey, model => model.Rename(command.NewName));
        public Task HandleAsync(ChangeCategoryDescription command) => WithCommand(command.Key).Execute(command.CategoryKey, model => model.ChangeDescription(command.Description));
        public Task HandleAsync(ChangeCategoryColor command) => WithCommand(command.Key).Execute(command.CategoryKey, model => model.ChangeColor(command.Color));
        public Task HandleAsync(ChangeCategoryIcon command) => WithCommand(command.Key).Execute(command.CategoryKey, model => model.ChangeIcon(command.Icon));
        public Task HandleAsync(DeleteCategory command) => WithCommand(command.Key).Execute(command.CategoryKey, model => model.Delete());
    }
}
