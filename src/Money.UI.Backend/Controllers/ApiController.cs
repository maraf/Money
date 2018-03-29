using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Money.Models.Api;
using Money.Services;
using Neptuo;
using Neptuo.Commands;
using Neptuo.Formatters;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Money.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ApiController : Controller
    {
        private readonly Formatters formatters;
        private readonly ICommandDispatcher commandDispatcher;
        private readonly IQueryDispatcher queryDispatcher;

        public ApiController(Formatters formatters, ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            Ensure.NotNull(formatters, "formatters");
            Ensure.NotNull(commandDispatcher, "commandDispatcher");
            Ensure.NotNull(queryDispatcher, "queryDispatcher");
            this.formatters = formatters;
            this.commandDispatcher = commandDispatcher;
            this.queryDispatcher = queryDispatcher;
        }

        [HttpPost]
        public ActionResult Query(Request request)
        {
            string payload = request.Payload;
            Type type = Type.GetType(request.Type);
            object query = formatters.Query.Deserialize(type, payload);

            MethodInfo methodInfo = queryDispatcher.GetType().GetMethod(nameof(queryDispatcher.QueryAsync));
            if (methodInfo != null)
            {
                methodInfo = methodInfo.MakeGenericMethod(type.GetInterfaces().First().GetGenericArguments().First());
                Task task = (Task)methodInfo.Invoke(queryDispatcher, new[] { query });
                task.Wait();

                object output = task.GetType().GetProperty(nameof(Task<object>.Result)).GetValue(task);
                if (output != null)
                {
                    payload = formatters.Query.Serialize(output);
                    type = output.GetType();

                    return Json(new Response()
                    {
                        Payload = payload,
                        Type = type.AssemblyQualifiedName
                    });
                }
            }

            return NotFound();
        }
    }
}
