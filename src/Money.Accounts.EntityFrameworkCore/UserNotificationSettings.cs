namespace Money;

public class UserNotificationSettings
{
    public string UserId { get; set; }
    public bool IsEnabled { get; set; }

    public User User { get; set; }
}
