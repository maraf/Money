using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Money.Accounts.Models;
using Money.Services;
using Neptuo;
using Neptuo.Activators;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Money.Accounts.Controllers;

[Authorize]
[Route("api/notifications")]
[ApiController]
public class NotificationsController : ControllerBase
{
    private readonly IFactory<AccountContext> dbFactory;
    private readonly PushNotificationSender pushSender;
    private readonly Json json;

    public NotificationsController(IFactory<AccountContext> dbFactory, PushNotificationSender pushSender, Json json)
    {
        Ensure.NotNull(dbFactory, "dbFactory");
        Ensure.NotNull(pushSender, "pushSender");
        Ensure.NotNull(json, "json");
        this.dbFactory = dbFactory;
        this.pushSender = pushSender;
        this.json = json;
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

    [HttpGet]
    public async Task<IActionResult> GetSettings()
    {
        string userId = GetUserId();
        using var db = dbFactory.Create();

        var settings = await db.UserNotificationSettings.FindAsync(userId);
        var expenseSettings = await db.UserNotificationExpenseTemplateSettings.FindAsync(userId);
        var hasSubscription = await db.PushSubscriptions.AnyAsync(s => s.UserId == userId && s.RevokedAt == null);

        var model = new NotificationSettingsModel
        {
            IsEnabled = settings?.IsEnabled ?? false,
            HasSubscription = hasSubscription,
            PushPublicKey = pushSender.PublicKey,
            ExpenseTemplates = new ExpenseTemplateNotificationSettingsModel
            {
                IsEnabled = expenseSettings?.IsEnabled ?? false,
                PreferredHour = expenseSettings?.PreferredHour ?? 8,
                TimeZone = expenseSettings?.TimeZone ?? "UTC"
            }
        };

        return Content(json.Serialize(model), "text/json");
    }

    [HttpPut]
    public async Task<IActionResult> PutSettings([FromBody] NotificationSettingsRequest request)
    {
        string userId = GetUserId();
        using var db = dbFactory.Create();

        var settings = await db.UserNotificationSettings.FindAsync(userId);
        if (settings == null)
        {
            settings = new UserNotificationSettings { UserId = userId };
            db.UserNotificationSettings.Add(settings);
        }

        settings.IsEnabled = request.IsEnabled;
        await db.SaveChangesAsync();
        return Ok();
    }

    [HttpPut("expense-templates")]
    public async Task<IActionResult> PutExpenseTemplateSettings([FromBody] ExpenseTemplateNotificationSettingsRequest request)
    {
        string userId = GetUserId();
        using var db = dbFactory.Create();

        var expenseSettings = await db.UserNotificationExpenseTemplateSettings.FindAsync(userId);
        if (expenseSettings == null)
        {
            expenseSettings = new UserNotificationExpenseTemplateSettings { UserId = userId };
            db.UserNotificationExpenseTemplateSettings.Add(expenseSettings);
        }

        expenseSettings.IsEnabled = request.IsEnabled;
        expenseSettings.PreferredHour = request.PreferredHour;
        expenseSettings.TimeZone = request.TimeZone;
        await db.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("subscriptions")]
    public async Task<IActionResult> Subscribe([FromBody] PushSubscriptionModel model)
    {
        string userId = GetUserId();
        using var db = dbFactory.Create();

        var existing = await db.PushSubscriptions.FirstOrDefaultAsync(s => s.Endpoint == model.Endpoint);
        if (existing != null)
        {
            existing.P256dh = model.P256dh;
            existing.Auth = model.Auth;
            existing.UserId = userId;
            existing.RevokedAt = null;
        }
        else
        {
            db.PushSubscriptions.Add(new PushSubscription
            {
                UserId = userId,
                Endpoint = model.Endpoint,
                P256dh = model.P256dh,
                Auth = model.Auth,
                CreatedAt = DateTime.UtcNow
            });
        }

        await db.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("subscriptions")]
    public async Task<IActionResult> Unsubscribe([FromBody] PushSubscriptionEndpointRequest request)
    {
        string userId = GetUserId();
        using var db = dbFactory.Create();

        var subscription = await db.PushSubscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId && s.Endpoint == request.Endpoint);

        if (subscription != null)
        {
            subscription.RevokedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
        }

        return Ok();
    }
}
