using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Events.Internals
{
    internal class ThreeBranchStorage
    {
        private Dictionary<Type, List<object>> directHandlers;
        private Dictionary<Type, List<object>> envelopeHandlers;
        private Dictionary<Type, List<object>> contextHandlers;

        #region Add handlers

        private void AddHandlerInternal(Type eventType, Dictionary<Type, List<object>> storage, object handler)
        {
            List<object> handlers;
            if (!storage.TryGetValue(eventType, out handlers))
                storage[eventType] = handlers = new List<object>();

            handlers.Add(handler);
        }

        public void AddDirectHandler(Type eventType, object handler)
        {
            if (directHandlers == null)
                directHandlers = new Dictionary<Type, List<object>>();

            AddHandlerInternal(eventType, directHandlers, handler);
        }

        public void AddEnvelopeHandler(Type eventType, object handler)
        {
            if (envelopeHandlers == null)
                envelopeHandlers = new Dictionary<Type, List<object>>();

            AddHandlerInternal(eventType, envelopeHandlers, handler);
        }

        public void AddContextHandler(Type eventType, object handler)
        {
            if (contextHandlers == null)
                contextHandlers = new Dictionary<Type, List<object>>();

            AddHandlerInternal(eventType, contextHandlers, handler);
        }

        #endregion

        #region Removing handlers

        private void RemoveHandlerInternal(Type eventType, Dictionary<Type, List<object>> storage, object handler)
        {
            if (storage != null)
            {
                List<object> handlers;
                if (storage.TryGetValue(eventType, out handlers))
                    handlers.Remove(handler);
            }
        }

        public void RemoveDirectHandler(Type eventType, object handler)
        {
            RemoveHandlerInternal(eventType, directHandlers, handler);
        }

        public void RemoveEnvelopeHandler(Type eventType, object handler)
        {
            RemoveHandlerInternal(eventType, envelopeHandlers, handler);
        }

        public void RemoveContextHandler(Type eventType, object handler)
        {
            RemoveHandlerInternal(eventType, contextHandlers, handler);
        }

        #endregion

        #region Reading handlers

        private IEnumerable<object> GetHandlersInternal(Type eventType, Dictionary<Type, List<object>> storage, bool includeSubTypes)
        {
            if (storage != null)
            {
                List<object> handlers;
                if (storage.TryGetValue(eventType, out handlers))
                    return handlers;
            }
                
            return Enumerable.Empty<object>();
        }

        public object[] GetDirectHandlers(Type eventType)
        {
            return GetHandlersInternal(eventType, directHandlers, true).ToArray();
        }

        public object[] GetEnvelopeHandlers(Type eventType)
        {
            return GetHandlersInternal(eventType, envelopeHandlers, true).ToArray();
        }

        public object[] GetContextHandlers(Type eventType)
        {
            return GetHandlersInternal(eventType, contextHandlers, true).ToArray();
        }

        #endregion
    }
}
