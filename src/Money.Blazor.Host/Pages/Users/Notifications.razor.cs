using Microsoft.AspNetCore.Components;
using Money.Accounts.Models;
using Money.Services;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Money.Pages.Users;

public partial class Notifications
{
    [Inject] protected PushNotificationInterop PushInterop { get; set; }
    [Inject] protected HttpClient Http { get; set; }
    [Inject] protected Json Json { get; set; }

    protected bool IsLoading { get; set; } = true;
    protected bool IsSupported { get; set; }
    protected bool IsEnabled { get; set; }
    protected bool ExpenseTemplatesEnabled { get; set; }
    protected int PreferredHour { get; set; } = 8;
    protected string TimeZone { get; set; } = "UTC";
    protected string Permission { get; set; } = "default";
    protected bool HasSubscription { get; set; }
    protected string PushPublicKey { get; set; }
    protected string StatusMessage { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            IsSupported = await PushInterop.IsSupportedAsync();

            if (IsSupported)
            {
                Permission = await PushInterop.GetPermissionAsync();
                TimeZone = await PushInterop.GetTimeZoneAsync();

                await LoadSettingsAsync();

                var currentSub = await PushInterop.GetSubscriptionAsync();
                HasSubscription = currentSub != null;
            }

            IsLoading = false;
            StateHasChanged();
        }
    }

    private async Task LoadSettingsAsync()
    {
        try
        {
            var response = await Http.GetAsync("/api/notifications");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var settings = Json.Deserialize<NotificationSettingsModel>(content);
                IsEnabled = settings.IsEnabled;
                PushPublicKey = settings.PushPublicKey;
                ExpenseTemplatesEnabled = settings.ExpenseTemplates?.IsEnabled ?? false;
                PreferredHour = settings.ExpenseTemplates?.PreferredHour ?? 8;
                if (!string.IsNullOrEmpty(settings.ExpenseTemplates?.TimeZone) && settings.ExpenseTemplates.TimeZone != "UTC")
                    TimeZone = settings.ExpenseTemplates.TimeZone;
            }
        }
        catch
        {
            // Settings not available yet
        }
    }

    private async Task OnGlobalToggleChanged(ChangeEventArgs e)
    {
        IsEnabled = (bool)e.Value;
        await SaveGlobalSettingsAsync();
    }

    private async Task OnExpenseTemplateToggleChanged(ChangeEventArgs e)
    {
        ExpenseTemplatesEnabled = (bool)e.Value;
        await SaveExpenseTemplateSettingsAsync();
    }

    private async Task OnPreferredHourChanged(ChangeEventArgs e)
    {
        PreferredHour = int.Parse(e.Value.ToString());
        await SaveExpenseTemplateSettingsAsync();
    }

    private async Task SaveGlobalSettingsAsync()
    {
        var payload = Json.Serialize(new { IsEnabled });
        await Http.PutAsync("/api/notifications", new StringContent(payload, Encoding.UTF8, "text/json"));
    }

    private async Task SaveExpenseTemplateSettingsAsync()
    {
        var payload = Json.Serialize(new { IsEnabled = ExpenseTemplatesEnabled, PreferredHour, TimeZone });
        await Http.PutAsync("/api/notifications/expense-templates", new StringContent(payload, Encoding.UTF8, "text/json"));
    }

    private async Task OnSubscribe()
    {
        StatusMessage = null;
        try
        {
            if (string.IsNullOrEmpty(PushPublicKey))
            {
                StatusMessage = "Push notifications are not configured on the server.";
                return;
            }

            var subscription = await PushInterop.SubscribeAsync(PushPublicKey);
            if (subscription != null)
            {
                var payload = Json.Serialize(new { subscription.Endpoint, subscription.P256dh, subscription.Auth });
                await Http.PostAsync("/api/notifications/subscriptions", new StringContent(payload, Encoding.UTF8, "text/json"));
                HasSubscription = true;
                Permission = await PushInterop.GetPermissionAsync();
                StatusMessage = "Successfully subscribed to notifications.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to subscribe: {ex.Message}";
        }
    }

    private async Task OnUnsubscribe()
    {
        StatusMessage = null;
        try
        {
            var endpoint = await PushInterop.UnsubscribeAsync();
            if (endpoint != null)
            {
                var payload = Json.Serialize(new { Endpoint = endpoint });
                var request = new HttpRequestMessage(HttpMethod.Delete, "/api/notifications/subscriptions")
                {
                    Content = new StringContent(payload, Encoding.UTF8, "text/json")
                };
                await Http.SendAsync(request);
            }
            HasSubscription = false;
            StatusMessage = "Unsubscribed from notifications.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to unsubscribe: {ex.Message}";
        }
    }
}
