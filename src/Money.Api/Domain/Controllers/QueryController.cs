using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Money.Api.Routing;
using Money.Services;
using Neptuo.Commands;
using Neptuo.Queries;
using Neptuo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Money.Controllers;
using Neptuo.Formatters;
using Neptuo.Models;
using System.Reflection;
using Money.Api.Models;

namespace Money.Domain.Controllers;

[Authorize]
[ApiController]
public class QueryController : Controller
{
    private readonly FormatterContainer formatters;
    private readonly IQueryDispatcher queryDispatcher;
    private readonly QueryMapper queryMapper;
    private readonly Json json;

    public QueryController(FormatterContainer formatters, IQueryDispatcher queryDispatcher, QueryMapper queryMapper, Json json)
    {
        Ensure.NotNull(formatters, "formatters");
        Ensure.NotNull(queryDispatcher, "queryDispatcher");
        Ensure.NotNull(queryMapper, "queryMapper");
        Ensure.NotNull(json, "json");
        this.formatters = formatters;
        this.queryDispatcher = queryDispatcher;
        this.queryMapper = queryMapper;
        this.json = json;
    }

    [HttpPost]
    [Route("/api/queries/{*url}")]
    public async Task<ActionResult> PostAsync(string url, JObject rawQuery)
    {
        string payload = rawQuery.ToString();
        Type type = queryMapper.FindTypeByUrl(url);
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
                if (output != null)
                {
                    type = output.GetType();

                    if (formatters.PlainTypes.Contains(type))
                        payload = output.ToString();
                    else
                        payload = formatters.Query.Serialize(output);
                }
                else
                {
                    payload = null;
                }

                return Content(payload, "text/json", Encoding.UTF8);
            }

            return NotFound();
        }
        catch (AggregateRootException e)
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
    }
}
