using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Queries
{
    partial class HttpQueryDispatcher
    {
        public delegate Task<object> Next(object query);

        public interface IMiddleware
        {
            Task<object> ExecuteAsync(object query, HttpQueryDispatcher dispatcher, Next next);
        }
    }
}
