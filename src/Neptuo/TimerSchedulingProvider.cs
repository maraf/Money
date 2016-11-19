using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Neptuo
{
    /// <summary>
    /// The implentation of <see cref="ISchedulingProvider"/> based on the <see cref="Timer"/>.
    /// When scheduling context should be executed after <see cref="longRunnerThreshold"/>, it uses single timer
    /// to check the queue every <see cref="longRunnerThreshold"/> and re-evaluate if the contexts should be scheduled.
    /// </summary>
    public partial class TimerSchedulingProvider : ISchedulingProvider
    {
        private readonly IDateTimeProvider dateTimeProvider;

        private readonly object timersLock = new object();
        private readonly List<Tuple<Timer, ISchedulingContext>> timers = new List<Tuple<Timer, ISchedulingContext>>();

        private readonly TimeSpan longRunnerThreshold;
        private readonly object longRunnersLock = new object();
        private List<ISchedulingContext> longRunners = null;
        private Timer longRunnerTimer;

        /// <summary>
        /// Creates new instance that uses <c>1d</c> to threshold 'long runnner' contexts.
        /// </summary>
        /// <param name="dateTimeProvider">The provider of execution delay.</param>
        public TimerSchedulingProvider(IDateTimeProvider dateTimeProvider)
            : this(dateTimeProvider, TimeSpan.FromDays(1))
        { }

        /// <summary>
        /// Creates new instance that uses <paramref name="longRunnerThreshold"/> to threshold 'long runnner' contexts. 
        /// </summary>
        /// <param name="dateTimeProvider">The provider of execution delay.</param>
        /// <param name="longRunnerThreshold">The timespan to be used as threshold when deciding whether the context is 'long runner'.</param>
        public TimerSchedulingProvider(IDateTimeProvider dateTimeProvider, TimeSpan longRunnerThreshold)
        {
            Ensure.NotNull(dateTimeProvider, "dateTimeProvider");

            if (longRunnerThreshold < TimeSpan.Zero)
                throw Ensure.Exception.ArgumentOutOfRange("longRunnerThreshold", "Long runner threshold must be between 0 and 1 day.");

            this.dateTimeProvider = dateTimeProvider;
            this.longRunnerThreshold = longRunnerThreshold;
        }

        public bool IsLaterExecutionRequired(Envelope envelope)
        {
            if (envelope == null)
                return false;

            DateTime executeAt;
            if (!envelope.TryGetExecuteAt(out executeAt))
                return false;

            TimeSpan delay = dateTimeProvider.GetExecutionDelay(executeAt);
            return delay > TimeSpan.Zero;
        }

        public void Schedule(ISchedulingContext context)
        {
            Ensure.NotNull(context, "context");

            DateTime executeAt;
            if (context.Envelope.TryGetExecuteAt(out executeAt))
            {
                TimeSpan delay = dateTimeProvider.GetExecutionDelay(executeAt);
                if (delay > longRunnerThreshold)
                {
                    ScheduleLongRunner(context);
                }
                else if (delay > TimeSpan.Zero)
                {
                    Timer timer = new Timer(
                        OnScheduled,
                        context,
                        delay,
                        TimeSpan.FromMilliseconds(-1)
                    );

                    lock (timersLock)
                        timers.Add(new Tuple<Timer, ISchedulingContext>(timer, context));
                }
                else
                {
                    // This should be handled by the calling infrastructure.
                    context.Execute();
                }
            }
            else
            {
                // This should be handled by the calling infrastructure.
                context.Execute();
            }
        }

        /// <summary>
        /// When the context should be executed and removed from the <see cref="timers"/>.
        /// </summary>
        /// <param name="state">The context to be executed.</param>
        private void OnScheduled(object state)
        {
            ISchedulingContext context = (ISchedulingContext)state;

            lock (timersLock)
            {
                Tuple<Timer, ISchedulingContext> item = timers.FirstOrDefault(t => t.Item2 == context);
                if (item != null)
                    timers.Remove(item);
            }

            context.Execute();
        }

        /// <summary>
        /// Enqueues <paramref name="context"/> to be scheduled as 'long runnner'.
        /// </summary>
        /// <param name="context">The context to be scheduled as 'long runner'</param>
        private void ScheduleLongRunner(ISchedulingContext context)
        {
            if (longRunners == null)
            {
                lock (longRunnersLock)
                {
                    if (longRunners == null)
                        longRunners = new List<ISchedulingContext>();

                    if (longRunnerTimer == null)
                    {
                        longRunnerTimer = new Timer(
                            OnLongRunnerScheduled,
                            null,
                            longRunnerThreshold,
                            TimeSpan.FromMilliseconds(-1)
                        );
                    }

                    longRunners.Add(context);
                }
            }
            else
            {
                lock (longRunnersLock)
                    longRunners.Add(context);
            }
        }

        /// <summary>
        /// Checks <see cref="longRunners"/> and re-evealuates if the contexts should be scheduled using regular timers.
        /// </summary>
        private void OnLongRunnerScheduled(object state)
        {
            List<ISchedulingContext> toSchedule = new List<ISchedulingContext>();
            lock (longRunnersLock)
            {
                if (longRunners == null)
                {
                    longRunnerTimer.Dispose();
                    longRunnerTimer = null;
                    return;
                }

                foreach (ISchedulingContext context in longRunners)
                {
                    DateTime executeAt;
                    if (context.Envelope.TryGetExecuteAt(out executeAt))
                    {
                        TimeSpan delay = dateTimeProvider.GetExecutionDelay(executeAt);
                        if (delay < longRunnerThreshold)
                            toSchedule.Add(context);
                    }
                }

                foreach (ISchedulingContext context in toSchedule)
                    longRunners.Remove(context);

                if (longRunners.Count == 0)
                {
                    longRunners = null;
                    longRunnerTimer.Dispose();
                    longRunnerTimer = null;
                }
            }

            foreach (ISchedulingContext context in toSchedule)
                Schedule(context);
        }
    }
}
