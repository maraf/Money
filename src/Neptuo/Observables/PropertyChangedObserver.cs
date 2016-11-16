using Neptuo.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Observables
{
    /// <summary>
    /// Decomposition of property changed handler.
    /// Providers methods for registering and unregistering handlers on single property change on single instance of model.
    /// On disposing subscription on <see cref="INotifyPropertyChanged.PropertyChanged"/> is released.
    /// </summary>
    /// <typeparam name="T">Type of model to observer properies of.</typeparam>
    public class PropertyChangedObserver<T> : DisposableBase
        where T : INotifyPropertyChanged
    {
        private readonly T model;
        private readonly Dictionary<string, List<Action<T>>> handlers = new Dictionary<string, List<Action<T>>>();

        /// <summary>
        /// Creates new instance listening on <paramref name="model"/>.
        /// </summary>
        /// <param name="model">Instance to listen on.</param>
        public PropertyChangedObserver(T model)
        {
            Ensure.NotNull(model, "model");
            this.model = model;
            this.model.PropertyChanged += OnModelPropertyChanged;
        }

        private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            List<Action<T>> list;
            if (handlers.TryGetValue(e.PropertyName, out list))
            {
                foreach (Action<T> handler in list)
                    handler(model);
            }
        }

        private List<Action<T>> GetHandlers(string propertyName)
        {
            List<Action<T>> list;
            if (!handlers.TryGetValue(propertyName, out list))
                handlers[propertyName] = list = new List<Action<T>>();

            return list;
        }

        /// <summary>
        /// Adds <paramref name="handler"/> to be executed property change <paramref name="propertyName"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="handler">The handler to be executed on <paramref name="propertyName"/> change.</param>
        /// <returns>Self (for fluency).</returns>
        public PropertyChangedObserver<T> Add(string propertyName, Action<T> handler)
        {
            Ensure.NotNullOrEmpty(propertyName, "propertyName");
            Ensure.NotNull(handler, "handler");

            List<Action<T>> list = GetHandlers(propertyName);
            list.Add(handler);
            return this;
        }

        /// <summary>
        /// Adds <paramref name="handler"/> to be executed property change <paramref name="propertyExpression"/>.
        /// </summary>
        /// <param name="propertyExpression">The expression pointing to the property.</param>
        /// <param name="handler">The handler to be executed on <paramref name="propertyExpression"/> change.</param>
        /// <returns>Self (for fluency).</returns>
        public PropertyChangedObserver<T> Add(Expression<Func<T, object>> propertyExpression, Action<T> handler)
        {
            Ensure.NotNull(propertyExpression, "propertyExpression");
            string propertyName = TypeHelper.PropertyName(propertyExpression);
            return Add(propertyName, handler);
        }

        /// <summary>
        /// Removes <paramref name="handler"/> from listening on property change <paramref name="propertyName"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="handler">The handler to be removed from listening on <paramref name="propertyName"/> change.</param>
        /// <returns>Self (for fluency).</returns>
        public PropertyChangedObserver<T> Remove(string propertyName, Action<T> handler)
        {
            Ensure.NotNullOrEmpty(propertyName, "propertyName");
            Ensure.NotNull(handler, "handler");

            List<Action<T>> list = GetHandlers(propertyName);
            list.Remove(handler);
            return this;
        }

        /// <summary>
        /// Removes <paramref name="handler"/> from listening on property change <paramref name="propertyExpression"/>.
        /// </summary>
        /// <param name="propertyExpression">The expression pointing to the property.</param>
        /// <param name="handler">The handler to be removed from listening on <paramref name="propertyExpression"/> change.</param>
        /// <returns>Self (for fluency).</returns>
        public PropertyChangedObserver<T> Remove(Expression<Func<T, object>> propertyExpression, Action<T> handler)
        {
            Ensure.NotNull(propertyExpression, "propertyExpression");
            string propertyName = TypeHelper.PropertyName(propertyExpression);
            return Remove(propertyName, handler);
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();

            model.PropertyChanged -= OnModelPropertyChanged;
            handlers.Clear();
        }
    }
}
