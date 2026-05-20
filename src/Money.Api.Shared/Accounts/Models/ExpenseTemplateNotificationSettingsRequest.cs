namespace Money.Accounts.Models;

public class ExpenseTemplateNotificationSettingsRequest
{
    public bool IsEnabled { get; set; }
    public int PreferredHour { get; set; }
    public string TimeZone { get; set; }
}
