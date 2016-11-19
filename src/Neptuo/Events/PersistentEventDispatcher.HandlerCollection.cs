using Neptuo.Events.Handlers;
using Neptuo.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Events
{
    partial class PersistentEventDispatcher
    {
        internal class HandlerCollection : IEventHandlerCollection
        {
            private readonly Dictionary<Type, HashSet<HandlerDescriptor>> storage;
            private readonly HandlerDescriptorProvider descriptorProvider;

            public HandlerCollection(Dictionary<Type, HashSet<HandlerDescriptor>> storage, HandlerDescriptorProvider descriptorProvider)
            {
                Ensure.NotNull(storage, "storage");
                Ensure.NotNull(descriptorProvider, "descriptorProvider");
                this.storage = storage;
                this.descriptorProvider = descriptorProvider;
            }

            public IEventHandlerCollection Add<TEvent>(IEventHandler<TEvent> handler)
            {
                Ensure.NotNull(handler, "handler");

                HandlerDescriptor descriptor = descriptorProvider.Get(handler, typeof(TEvent));
                HashSet<HandlerDescriptor> handlers;
                if (!storage.TryGetValue(descriptor.ArgumentType, out handlers))
                    storage[descriptor.ArgumentType] = handlers = new HashSet<HandlerDescriptor>();

                handlers.Add(descriptor);
                return this;
            }

            public IEventHandlerCollection Remove<TEvent>(IEventHandler<TEvent> handler)
            {
                Ensure.NotNull(handler, "handler");

                HandlerDescriptor descriptor = descriptorProvider.Get(handler, typeof(TEvent));
                HashSet<HandlerDescriptor> handlers;
                if (storage.TryGetValue(descriptor.ArgumentType, out handlers))
                    handlers.Remove(descriptor);

                return this;
            }

        }

    }
}
