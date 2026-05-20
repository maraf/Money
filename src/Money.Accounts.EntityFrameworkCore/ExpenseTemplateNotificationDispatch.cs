using System;

namespace Money;

public class ExpenseTemplateNotificationDispatch
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public Guid ExpenseTemplateId { get; set; }
    public DateTime Date { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }

    public User User { get; set; }
}
