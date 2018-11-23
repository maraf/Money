using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Money.Commands;
using Money.Data;
using Money.Models;
using Neptuo;
using Neptuo.Activators;
using Neptuo.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IFactory<EventSourcingContext> eventSourcingFactory;
        private readonly IFactory<ReadModelContext> readModelsFactory;
        private readonly ICommandDispatcher commands;

        public AdminController(UserManager<ApplicationUser> userManager, IFactory<EventSourcingContext> eventSourcingFactory, IFactory<ReadModelContext> readModelsFactory, ICommandDispatcher commands)
        {
            Ensure.NotNull(userManager, "userManager");
            Ensure.NotNull(readModelsFactory, "readModelsFactory");
            Ensure.NotNull(eventSourcingFactory, "eventSourcingFactory");
            Ensure.NotNull(commands, "commands");
            this.userManager = userManager;
            this.eventSourcingFactory = eventSourcingFactory;
            this.readModelsFactory = readModelsFactory;
            this.commands = commands;
        }

        public async Task<IActionResult> RecreateDemo()
        {
            if (Request.Host.Host != "localhost" && Request.Host.Host != "127.0.0.1")
                return NotFound();

            ApplicationUser demoUser = await userManager.FindByNameAsync(ClaimsPrincipalExtensions.DemoUserName);
            if (demoUser != null)
            {
                // Clear

                await userManager.DeleteAsync(demoUser);

                List<Guid> outcomeIds = null;
                using (ReadModelContext readModels = readModelsFactory.Create())
                {
                    readModels.Categories.RemoveRange(readModels.Categories.Where(c => c.UserId == demoUser.Id));
                    readModels.Currencies.RemoveRange(readModels.Currencies.Where(c => c.UserId == demoUser.Id));

                    var outcomes = readModels.Outcomes.Where(c => c.UserId == demoUser.Id);
                    outcomeIds = outcomes.Select(o => o.Id).ToList();
                    readModels.Outcomes.RemoveRange(outcomes);

                    await readModels.SaveChangesAsync();
                }

                using (EventSourcingContext eventSourcing = eventSourcingFactory.Create())
                {
                    var events = eventSourcing.Events.Where(e => outcomeIds.Contains(e.AggregateID));
                    eventSourcing.Events.RemoveRange(events);

                    await eventSourcing.SaveChangesAsync();
                }
            }

            // Recreate
            await ApplicationDataSeeder.InitializeAsync(userManager, commands);

            return Ok("Recreated.");
        }
    }
}
