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
using Neptuo.Formatters;
using System.Reflection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Neptuo.Models.Keys;
using IKey = Neptuo.Models.Keys.IKey;

namespace Money.Domain.Controllers;

[Authorize]
[ApiController]
public class CommandController : Controller
{
    private readonly FormatterContainer formatters;
    private readonly ICommandDispatcher commandDispatcher;
    private readonly CommandMapper commandMapper;
    private readonly Json json;

    public CommandController(FormatterContainer formatters, ICommandDispatcher commandDispatcher, CommandMapper commandMapper, Json json)
    {
        Ensure.NotNull(formatters, "formatters");
        Ensure.NotNull(commandDispatcher, "commandDispatcher");
        Ensure.NotNull(commandMapper, "commandMapper");
        Ensure.NotNull(json, "json");
        this.formatters = formatters;
        this.commandDispatcher = commandDispatcher;
        this.commandMapper = commandMapper;
        this.json = json;
    }

    [HttpPost]
    [Route("/api/commands/{url}")]
    public async Task<ActionResult> PostAsync(string url, JObject rawQuery)
    {
        string payload = rawQuery.ToString();
        Type type = commandMapper.FindTypeByUrl(url);
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

    [HttpGet]
    [Route("/api/commands/{url}/{guid}")]
    public async Task<ActionResult> GetAsync(string url, string guid)
    {
        if (Guid.TryParse(guid, out var id))
        {
            Type type = commandMapper.FindTypeByUrl(url);
            IKey commandKey = GuidKey.Create(id, KeyFactory.Empty(type).Type);

            // TODO: Query command store.
        }

        return NotFound();
    }
}
