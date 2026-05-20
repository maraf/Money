using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Money.Accounts.Models;
using Neptuo;
using Neptuo.Activators;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebPush;

namespace Money.Accounts;

public class PushNotificationSender
{
    private readonly IFactory<AccountContext> dbFactory;
    private readonly IOptions<NotificationOptions> options;
    private readonly ILogger<PushNotificationSender> logger;

    public bool IsConfigured => !string.IsNullOrEmpty(options.Value.PublicKey) && !string.IsNullOrEmpty(options.Value.PrivateKey);
    public string PublicKey => options.Value.PublicKey;

    public PushNotificationSender(IFactory<AccountContext> dbFactory, IOptions<NotificationOptions> options, ILogger<PushNotificationSender> logger)
    {
        Ensure.NotNull(dbFactory, "dbFactory");
        Ensure.NotNull(options, "options");
        Ensure.NotNull(logger, "logger");
        this.dbFactory = dbFactory;
        this.options = options;
        this.logger = logger;
    }

    public async Task SendAsync(string userId, string title, string body, string url, string tag)
    {
        if (!IsConfigured)
        {
            logger.LogWarning("Push notifications are not configured (missing VAPID keys).");
            return;
        }

        using var db = dbFactory.Create();
        var subscriptions = await db.PushSubscriptions
            .Where(s => s.UserId == userId && s.RevokedAt == null)
            .ToListAsync();

        if (subscriptions.Count == 0)
            return;

        var vapidDetails = new VapidDetails(options.Value.Subject, options.Value.PublicKey, options.Value.PrivateKey);
        var client = new WebPushClient();

        var payload = System.Text.Json.JsonSerializer.Serialize(new { title, body, url, tag });

        foreach (var subscription in subscriptions)
        {
            try
            {
                var pushSubscription = new WebPush.PushSubscription(subscription.Endpoint, subscription.P256dh, subscription.Auth);
                await client.SendNotificationAsync(pushSubscription, payload, vapidDetails);
            }
            catch (WebPushException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Gone || ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                logger.LogInformation("Push subscription {Endpoint} is gone, revoking.", subscription.Endpoint);
                subscription.RevokedAt = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send push notification to {Endpoint}.", subscription.Endpoint);
            }
        }

        await db.SaveChangesAsync();
    }
}
