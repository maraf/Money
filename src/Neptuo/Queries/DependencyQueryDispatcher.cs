using Neptuo.Activators;
using Neptuo.Linq.Expressions;
using Neptuo.Queries.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Queries
{
    /// <summary>
    /// Implementation of <see cref="IQueryDispatcher"/> which uses <see cref="IDependencyProvider"/> to read registrations from.
    /// </summary>
    public class DependencyQueryDispatcher : IQueryDispatcher
    {
        private static string handleAsyncMethodName = "HandleAsync"; //TypeHelper.MethodName<IQueryHandler<IQuery<object>, object>, IQuery<object>, object>(q => q.HandleAsync);
        private IDependencyProvider dependencyProvider;
        
        /// <summary>
        /// Creates new instance with <paramref name="dependencyProvider"/>.
        /// </summary>
        /// <param name="dependencyProvider">Source for registrations.</param>
        public DependencyQueryDispatcher(IDependencyProvider dependencyProvider)
        {
            Ensure.NotNull(dependencyProvider, "dependencyProvider");
            this.dependencyProvider = dependencyProvider;
        }

        public Task<TOutput> QueryAsync<TOutput>(IQuery<TOutput> query)
        {
            Ensure.NotNull(query, "query");

            Type handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TOutput));
            object handler = dependencyProvider.Resolve(handlerType);

            MethodInfo method = handlerType.GetMethod(handleAsyncMethodName);
            return (Task<TOutput>)method.Invoke(handler, new object[] { handler });
        }
    }
}
