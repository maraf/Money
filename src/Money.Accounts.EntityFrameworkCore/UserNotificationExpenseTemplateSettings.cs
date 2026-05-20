namespace Money;

public class UserNotificationExpenseTemplateSettings
{
    public string UserId { get; set; }
    public bool IsEnabled { get; set; }
    public int PreferredHour { get; set; }
    public string TimeZone { get; set; }

    public User User { get; set; }
}
