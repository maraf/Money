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
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Money.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ApiController : Controller
    {
        private readonly FormatterContainer formatters;
        private readonly ICommandDispatcher commandDispatcher;
        private readonly IQueryDispatcher queryDispatcher;

        public ApiController(FormatterContainer formatters, ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            Ensure.NotNull(formatters, "formatters");
            Ensure.NotNull(commandDispatcher, "commandDispatcher");
            Ensure.NotNull(queryDispatcher, "queryDispatcher");
            this.formatters = formatters;
            this.commandDispatcher = commandDispatcher;
            this.queryDispatcher = queryDispatcher;
        }

        public string UserName() => HttpContext.User.Identity.Name;

        [HttpPost]
        public ActionResult Query([FromBody] Request request)
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
                    ResponseType responseType = ResponseType.Composite;
                    type = output.GetType();

                    if (output is string || output is int || output is decimal || output is bool)
                    {
                        payload = output.ToString();
                        responseType = ResponseType.Plain;
                    }
                    else
                    {
                        payload = formatters.Query.Serialize(output);
                    }

                    HttpContext.Response.ContentType = "text/json";
                    return Json(new Response()
                    {
                        payload = payload,
                        type = type.AssemblyQualifiedName,
                        responseType = responseType
                    });
                }
            }

            return NotFound();
        }

        public ActionResult Command([FromBody] Request request)
        {
            string payload = request.Payload;
            Type type = Type.GetType(request.Type);
            object command = formatters.Command.Deserialize(type, payload);

            MethodInfo methodInfo = commandDispatcher.GetType().GetMethod(nameof(commandDispatcher.HandleAsync));
            if (methodInfo != null)
            {
                methodInfo = methodInfo.MakeGenericMethod(type);
                Task task = (Task)methodInfo.Invoke(commandDispatcher, new[] { command });
                task.Wait();

                return Ok();
            }

            return StatusCode(500);
        }
    }
}
