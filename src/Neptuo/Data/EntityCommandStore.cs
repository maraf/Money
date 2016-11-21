using Microsoft.EntityFrameworkCore;
using Neptuo.Data.Entity;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Data
{
    public class EntityCommandStore : ICommandStore, ICommandPublishingStore
    {
        private readonly ICommandContext context;

        public EntityCommandStore(ICommandContext context)
        {
            Ensure.NotNull(context, "context");
            this.context = context;
        }

        public void Save(CommandModel command)
        {
            Ensure.NotNull(command, "command");

            CommandEntity entity = CommandEntity.FromModel(command);
            context.Commands.Add(entity);
            context.UnPublishedCommands.Add(new UnPublishedCommandEntity(entity));

            context.Save();
        }

        public void Save(IEnumerable<CommandModel> commands)
        {
            Ensure.NotNull(commands, "commands");

            foreach (CommandEntity entity in commands.Select(CommandEntity.FromModel))
            {
                context.Commands.Add(entity);
                context.UnPublishedCommands.Add(new UnPublishedCommandEntity(entity));
            }

            context.Save();
        }

        public async Task<IEnumerable<CommandModel>> GetAsync()
        {
            return await context.UnPublishedCommands
                .Where(c => !c.IsHandled)
                .Select(c => c.Command.ToModel())
                .ToListAsync();
        }

        public Task PublishedAsync(IKey key)
        {
            GuidKey commandKey = key as GuidKey;
            if (commandKey == null)
                throw Ensure.Exception.NotGuidKey(commandKey.GetType(), "key");

            UnPublishedCommandEntity entity = context.UnPublishedCommands.FirstOrDefault(e => e.Command.CommandType == commandKey.Type && e.Command.CommandID == commandKey.Guid);
            if (entity == null)
                return Task.FromResult(true);

            entity.IsHandled = true;
            return context.SaveAsync();
        }

        public Task ClearAsync()
        {
            foreach (UnPublishedCommandEntity entity in context.UnPublishedCommands.Where(c => c.IsHandled))
                context.UnPublishedCommands.Remove(entity);

            return context.SaveAsync();
        }
    }
}
