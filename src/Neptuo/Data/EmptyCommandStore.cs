using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neptuo.Models.Keys;
using Neptuo.Threading.Tasks;
using Neptuo;

namespace Neptuo.Data
{
    /// <summary>
    /// The empty implementation of <see cref="ICommandStore"/> and <see cref="ICommandPublishingStore"/>.
    /// </summary>
    public class EmptyCommandStore : ICommandStore, ICommandPublishingStore
    {
        private readonly Method supportedMethod;
        private readonly ICommandStore store;
        private readonly ICommandPublishingStore publishingStore;

        /// <summary>
        /// Creates new instance where all methods are empty.
        /// </summary>
        public EmptyCommandStore()
        { }

        /// <summary>
        /// Creates new instance with enumeration of supported methods in <paramref name="supportedMethod"/>. Other methods are empty.
        /// If one of stores required by <paramref name="supportedMethod"/> is <c>null</c>, this method is automatically not supported.
        /// </summary>
        /// <param name="supportedMethod">The enumeration of supported methods.</param>
        /// <param name="store">The store for loading and saving commands.</param>
        /// <param name="publishingStore">The store for saving delivery information.</param>
        public EmptyCommandStore(Method supportedMethod, ICommandStore store, ICommandPublishingStore publishingStore)
        {
            this.supportedMethod = supportedMethod;
            this.store = store;
            this.publishingStore = publishingStore;
        }

        public void Save(IEnumerable<CommandModel> commands)
        {
            if ((supportedMethod & Method.SaveAll) == Method.SaveAll)
                store.Save(commands);
        }

        public void Save(CommandModel command)
        {
            if ((supportedMethod & Method.Save) == Method.Save)
                store.Save(command);
        }

        public Task ClearAsync()
        {
            if ((supportedMethod & Method.Clear) == Method.Clear)
                return publishingStore.ClearAsync();

            return Async.CompletedTask;
        }

        public Task<IEnumerable<CommandModel>> GetAsync()
        {
            if ((supportedMethod & Method.Get) == Method.Get)
                return publishingStore.GetAsync();

            return Task.FromResult(Enumerable.Empty<CommandModel>());
        }

        public Task PublishedAsync(IKey commandKey)
        {
            if ((supportedMethod & Method.Publish) == Method.Publish)
                return publishingStore.PublishedAsync(commandKey);

            return Async.CompletedTask;
        }

        /// <summary>
        /// The enumeration of supported methods.
        /// </summary>
        [Flags]
        public enum Method
        {
            /// <summary>
            /// None
            /// </summary>
            None = 0,

            /// <summary>
            /// <see cref="ICommandStore.Save(CommandModel)"/>.
            /// </summary>
            Save = 1,

            /// <summary>
            /// <see cref="ICommandStore.Save(IEnumerable{CommandModel})"/>.
            /// </summary>
            SaveAll = 2,

            /// <summary>
            /// <see cref="ICommandPublishingStore.ClearAsync"/>.
            /// </summary>
            Clear = 4,

            /// <summary>
            /// <see cref="ICommandPublishingStore.GetAsync"/>.
            /// </summary>
            Get = 8,

            /// <summary>
            /// <see cref="ICommandPublishingStore.PublishedAsync(IKey)"/>.
            /// </summary>
            Publish = 16
        }
    }
}
