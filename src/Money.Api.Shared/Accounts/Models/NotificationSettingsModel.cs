namespace Money.Accounts.Models;

public class NotificationSettingsModel
{
    public bool IsEnabled { get; set; }
    public bool HasSubscription { get; set; }
    public string PushPublicKey { get; set; }
    public ExpenseTemplateNotificationSettingsModel ExpenseTemplates { get; set; }
}
