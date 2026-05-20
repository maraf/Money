using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Money.Accounts.Models;
using Money.Models;
using Neptuo.Activators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Money.Accounts;

public class ExpenseTemplateNotificationBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IOptions<NotificationOptions> options;
    private readonly TimeProvider timeProvider;
    private readonly ILogger<ExpenseTemplateNotificationBackgroundService> logger;

    public ExpenseTemplateNotificationBackgroundService(
        IServiceScopeFactory scopeFactory,
        IOptions<NotificationOptions> options,
        TimeProvider timeProvider,
        ILogger<ExpenseTemplateNotificationBackgroundService> logger)
    {
        this.scopeFactory = scopeFactory;
        this.options = options;
        this.timeProvider = timeProvider;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await NotifyDueTemplatesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing expense template notifications.");
            }

            await Task.Delay(options.Value.ExpenseTemplates.TickInterval, stoppingToken);
        }
    }

    private async Task NotifyDueTemplatesAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var accountDbFactory = scope.ServiceProvider.GetRequiredService<IFactory<AccountContext>>();
        var readModelDbFactory = scope.ServiceProvider.GetRequiredService<IFactory<ReadModelContext>>();
        var pushSender = scope.ServiceProvider.GetRequiredService<PushNotificationSender>();

        if (!pushSender.IsConfigured)
            return;

        using var accountDb = accountDbFactory.Create();

        var candidates = await accountDb.UserNotificationExpenseTemplateSettings
            .Where(s => s.IsEnabled)
            .ToListAsync(ct);

        if (candidates.Count == 0)
            return;

        foreach (var candidate in candidates)
        {
            try
            {
                TimeZoneInfo tz;
                try
                {
                    tz = TimeZoneInfo.FindSystemTimeZoneById(candidate.TimeZone);
                }
                catch (TimeZoneNotFoundException)
                {
                    logger.LogWarning("Unknown timezone {TimeZone} for user {UserId}, skipping.", candidate.TimeZone, candidate.UserId);
                    continue;
                }

                var utcNow = timeProvider.GetUtcNow();
                var userLocalTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow.UtcDateTime, tz);
                if (userLocalTime.Hour != candidate.PreferredHour)
                    continue;

                var globalSettings = await accountDb.UserNotificationSettings.FindAsync(new object[] { candidate.UserId }, ct);
                if (globalSettings == null || !globalSettings.IsEnabled)
                    continue;

                var today = DateOnly.FromDateTime(userLocalTime);
                await SendNotificationsForUserAsync(candidate.UserId, today, accountDb, readModelDbFactory, pushSender, ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing notifications for user {UserId}.", candidate.UserId);
            }
        }
    }

    private async Task SendNotificationsForUserAsync(
        string userId,
        DateOnly today,
        AccountContext accountDb,
        IFactory<ReadModelContext> readModelDbFactory,
        PushNotificationSender pushSender,
        CancellationToken ct)
    {
        using var readModelDb = readModelDbFactory.Create();

        var templates = await readModelDb.ExpenseTemplates
            .Where(t => t.UserId == userId && !t.IsDeleted && t.Period != null)
            .ToListAsync(ct);

        var dueTemplates = templates.Where(t => IsDueToday(t, today)).ToList();
        if (dueTemplates.Count == 0)
            return;

        var alreadySent = await accountDb.ExpenseTemplateNotificationDispatches
            .Where(d => d.UserId == userId && d.Date == today.ToDateTime(TimeOnly.MinValue))
            .Select(d => d.ExpenseTemplateId)
            .ToListAsync(ct);

        var toNotify = dueTemplates.Where(t => !alreadySent.Contains(t.Id)).ToList();
        if (toNotify.Count == 0)
            return;

        var title = toNotify.Count == 1
            ? "Scheduled expense due today"
            : $"{toNotify.Count} scheduled expenses due today";

        var body = toNotify.Count == 1
            ? toNotify[0].Description ?? "An expense template is due today."
            : string.Join(", ", toNotify.Select(t => t.Description ?? "Unnamed").Take(3)) + (toNotify.Count > 3 ? $" +{toNotify.Count - 3} more" : "");

        await pushSender.SendAsync(userId, title, body, "/expense-templates", "expense-template-due");

        foreach (var template in toNotify)
        {
            accountDb.ExpenseTemplateNotificationDispatches.Add(new ExpenseTemplateNotificationDispatch
            {
                UserId = userId,
                ExpenseTemplateId = template.Id,
                Date = today.ToDateTime(TimeOnly.MinValue),
                CreatedAt = DateTime.UtcNow,
                SentAt = DateTime.UtcNow
            });
        }

        await accountDb.SaveChangesAsync(ct);
    }

    private static bool IsDueToday(ExpenseTemplateEntity template, DateOnly today)
    {
        return template.Period switch
        {
            RecurrencePeriod.Monthly => template.DayInPeriod == today.Day,
            RecurrencePeriod.Yearly => template.DayInPeriod == today.Day && template.MonthInPeriod == today.Month,
            RecurrencePeriod.Single => template.DueDate.HasValue && DateOnly.FromDateTime(template.DueDate.Value) == today,
            RecurrencePeriod.XMonths => IsXMonthsDue(template, today),
            _ => false
        };
    }

    private static bool IsXMonthsDue(ExpenseTemplateEntity template, DateOnly today)
    {
        if (template.DueDate == null || template.EveryXPeriods == null || template.DayInPeriod == null)
            return false;

        if (template.DayInPeriod != today.Day)
            return false;

        var startDate = DateOnly.FromDateTime(template.DueDate.Value);
        var monthsDiff = (today.Year - startDate.Year) * 12 + (today.Month - startDate.Month);

        return monthsDiff >= 0 && monthsDiff % template.EveryXPeriods.Value == 0;
    }
}
