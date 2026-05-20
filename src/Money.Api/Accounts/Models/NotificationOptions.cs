using System;

namespace Money.Accounts.Models;

public class NotificationOptions
{
    public string Subject { get; set; }
    public string PublicKey { get; set; }
    public string PrivateKey { get; set; }
    public ExpenseTemplateNotificationOptions ExpenseTemplates { get; set; } = new();
}

public class ExpenseTemplateNotificationOptions
{
    public TimeSpan TickInterval { get; set; } = TimeSpan.FromMinutes(15);
}
