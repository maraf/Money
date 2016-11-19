using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Internals
{
    internal class ScheduleCommandContext : ISchedulingContext
    {
        public HandlerDescriptor Handler { get; private set; }
        public ArgumentDescriptor Argument { get; private set; }
        public Envelope Envelope { get; private set; }

        private readonly Action<ScheduleCommandContext> execute;

        public ScheduleCommandContext(HandlerDescriptor handler, ArgumentDescriptor argument, Envelope envelope, Action<ScheduleCommandContext> execute)
        {
            Handler = handler;
            Argument = argument;
            Envelope = envelope;
            this.execute = execute;
        }

        public void Execute()
        {
            execute(this);
        }
    }
}
