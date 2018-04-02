using Neptuo;
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
        ICommandHandler<Envelope<CreateCategory>>, 
        ICommandHandler<Envelope<RenameCategory>>, 
        ICommandHandler<Envelope<ChangeCategoryDescription>>, 
        ICommandHandler<Envelope<ChangeCategoryColor>>, 
        ICommandHandler<Envelope<ChangeCategoryIcon>>, 
        ICommandHandler<Envelope<DeleteCategory>>
    {
        public CategoryHandler(IFactory<IRepository<Category, IKey>> repositoryFactory) 
            : base(repositoryFactory)
        { }

        public Task HandleAsync(Envelope<CreateCategory> envelope) => WithCommand(envelope.Body.Key).Execute(envelope, () => new Category(envelope.Body.Name, envelope.Body.Description, envelope.Body.Color));
        public Task HandleAsync(Envelope<RenameCategory> envelope) => WithCommand(envelope.Body.Key).Execute(envelope.Body.CategoryKey, envelope, model => model.Rename(envelope.Body.NewName));
        public Task HandleAsync(Envelope<ChangeCategoryDescription> envelope) => WithCommand(envelope.Body.Key).Execute(envelope.Body.CategoryKey, envelope, model => model.ChangeDescription(envelope.Body.Description));
        public Task HandleAsync(Envelope<ChangeCategoryColor> envelope) => WithCommand(envelope.Body.Key).Execute(envelope.Body.CategoryKey, envelope, model => model.ChangeColor(envelope.Body.Color));
        public Task HandleAsync(Envelope<ChangeCategoryIcon> envelope) => WithCommand(envelope.Body.Key).Execute(envelope.Body.CategoryKey, envelope, model => model.ChangeIcon(envelope.Body.Icon));
        public Task HandleAsync(Envelope<DeleteCategory> envelope) => WithCommand(envelope.Body.Key).Execute(envelope.Body.CategoryKey, envelope, model => model.Delete());
    }
}
