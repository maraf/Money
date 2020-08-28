using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Money.Api.Models;
using Money.Api.Routing;
using Money.Services;
using Neptuo;
using Neptuo.Commands;
using Neptuo.Formatters;
using Neptuo.Models;
using Neptuo.Queries;
using Newtonsoft.Json.Linq;
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
    [ApiController]
    [Route("[controller]/[action]")]
    public class ApiController : Controller
    {
        private static Type[] plainTypes = new[] { typeof(string), typeof(int), typeof(decimal), typeof(bool) };

        private readonly FormatterContainer formatters;
        private readonly ICommandDispatcher commandDispatcher;
        private readonly IQueryDispatcher queryDispatcher;
        private readonly CommandMapper commandMapper;
        private readonly QueryMapper queryMapper;
        private readonly Json json;

        public ApiController(FormatterContainer formatters, ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher, CommandMapper commandMapper, QueryMapper queryMapper, Json json)
        {
            Ensure.NotNull(formatters, "formatters");
            Ensure.NotNull(commandDispatcher, "commandDispatcher");
            Ensure.NotNull(queryDispatcher, "queryDispatcher");
            Ensure.NotNull(commandMapper, "commandMapper");
            Ensure.NotNull(queryMapper, "queryMapper");
            Ensure.NotNull(json, "json");
            this.formatters = formatters;
            this.commandDispatcher = commandDispatcher;
            this.queryDispatcher = queryDispatcher;
            this.commandMapper = commandMapper;
            this.queryMapper = queryMapper;
            this.json = json;
        }

        public string UserName() => HttpContext.User.Identity.Name;

        [HttpPost("query")]
        public Task<ActionResult> QueryAsync([FromBody] Request request)
        {
            Ensure.NotNull(request, "request");

            string payload = request.Payload;
            Type type = Type.GetType(request.Type);

            return QueryAsync(payload, type);
        }

        [HttpPost]
        [Route("{*url}")]
        public Task<ActionResult> QueryAsync(string url, JObject rawQuery)
        {
            Ensure.NotNullOrEmpty(url, "url");
            Ensure.NotNull(rawQuery, "rawQuery");

            Type type = queryMapper.FindTypeByUrl(url);
            return QueryAsync(rawQuery.ToString(), type);
        }

        private async Task<ActionResult> QueryAsync(string payload, Type type)
        {
            Ensure.NotNull(type, "type");

            object query = formatters.Query.Deserialize(type, payload);

            try
            {
                MethodInfo methodInfo = queryDispatcher.GetType().GetMethod(nameof(queryDispatcher.QueryAsync));
                if (methodInfo != null)
                {
                    methodInfo = methodInfo.MakeGenericMethod(type.GetInterfaces().First().GetGenericArguments().First());
                    Task task = (Task)methodInfo.Invoke(queryDispatcher, new[] { query });
                    await task;

                    object output = task.GetType().GetProperty(nameof(Task<object>.Result)).GetValue(task);
                    ResponseType responseType = ResponseType.Composite;
                    if (output != null)
                    {
                        type = output.GetType();

                        if (plainTypes.Contains(type))
                        {
                            payload = output.ToString();
                            responseType = ResponseType.Plain;
                        }
                        else
                        {
                            payload = formatters.Query.Serialize(output);
                        }
                    }
                    else
                    {
                        type = type.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQuery<>)).GetGenericArguments()[0];
                        payload = null;
                        if (plainTypes.Contains(type))
                            responseType = ResponseType.Plain;
                    }

                    return new ContentResult()
                    {
                        Content = json.Serialize(new Response()
                        {
                            Payload = payload,
                            Type = type.AssemblyQualifiedName,
                            ResponseType = responseType
                        }),
                        ContentType = "text/json"
                    };
                }

                return NotFound();
            }
            catch (AggregateRootException e)
            {
                return ProcessException(e);
            }
        }

        private ActionResult ProcessException(AggregateRootException e)
        {
            string exceptionType = e.GetType().AssemblyQualifiedName;
            string exceptionPayload = formatters.Exception.Serialize(e);

            var response = new Response()
            {
                Type = exceptionType,
                Payload = exceptionPayload
            };

            return new ContentResult()
            {
                Content = json.Serialize(response),
                StatusCode = 500,
                ContentType = "text/json"
            };
        }

        [HttpPost("command")]
        public Task<ActionResult> CommandAsync([FromBody] Request request)
        {
            string payload = request.Payload;
            Type type = Type.GetType(request.Type);

            return CommandAsync(payload, type);
        }

        [HttpPost]
        [Route("{*url}")]
        public Task<ActionResult> CommandAsync(string url, JObject rawCommand)
        {
            Ensure.NotNullOrEmpty(url, "url");
            Ensure.NotNull(rawCommand, "rawCommand");

            Type type = commandMapper.FindTypeByUrl(url);
            return CommandAsync(rawCommand.ToString(), type);
        }

        private async Task<ActionResult> CommandAsync(string payload, Type type)
        {
            object command = formatters.Command.Deserialize(type, payload);

            MethodInfo methodInfo = commandDispatcher.GetType().GetMethod(nameof(commandDispatcher.HandleAsync));
            if (methodInfo != null)
            {
                methodInfo = methodInfo.MakeGenericMethod(type);
                Task task = (Task)methodInfo.Invoke(commandDispatcher, new[] { command });
                await task;

                return Ok();
            }

            return StatusCode(500);
        }
    }
}
