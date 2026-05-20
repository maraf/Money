using Microsoft.JSInterop;
using Neptuo;
using System.Threading.Tasks;

namespace Money.Services;

public class PushNotificationInterop
{
    private readonly IJSRuntime js;

    public PushNotificationInterop(IJSRuntime js)
    {
        Ensure.NotNull(js, "js");
        this.js = js;
    }

    public async Task<bool> IsSupportedAsync()
        => await js.InvokeAsync<bool>("Money.Notifications.isSupported");

    public async Task<string> GetPermissionAsync()
        => await js.InvokeAsync<string>("Money.Notifications.getPermission");

    public async Task<string> GetTimeZoneAsync()
        => await js.InvokeAsync<string>("Money.Notifications.getTimeZone");

    public async Task<PushSubscriptionResult> GetSubscriptionAsync()
        => await js.InvokeAsync<PushSubscriptionResult>("Money.Notifications.getSubscription");

    public async Task<PushSubscriptionResult> SubscribeAsync(string publicKey)
        => await js.InvokeAsync<PushSubscriptionResult>("Money.Notifications.subscribe", publicKey);

    public async Task<string> UnsubscribeAsync()
        => await js.InvokeAsync<string>("Money.Notifications.unsubscribe");
}

public class PushSubscriptionResult
{
    public string Endpoint { get; set; }
    public string P256dh { get; set; }
    public string Auth { get; set; }
}
