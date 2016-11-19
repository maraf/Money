using Neptuo.Commands;
using Neptuo.Events;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Models.Domains
{
    /// <summary>
    /// The base class for (long running) process model.
    /// </summary>
    public class ProcessRoot : AggregateRoot, IProcessRoot
    {
        private readonly List<Envelope<ICommand>> commands = new List<Envelope<ICommand>>();

        /// <summary>
        /// The enumeration of unpublished commands.
        /// </summary>
        public IEnumerable<Envelope<ICommand>> Commands
        {
            get { return commands; }
        }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        protected ProcessRoot()
            : base()
        { }

        /// <summary>
        /// Creates new instance with explicitly defined <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of this (new) instance.</param>
        protected ProcessRoot(IKey key)
            : base(key)
        { }

        /// <summary>
        /// Loads instance with <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of this instance.</param>
        /// <param name="events">The enumeration of events describing current state.</param>
        protected ProcessRoot(IKey key, IEnumerable<IEvent> events)
            : base(key, events)
        { }

        /// <summary>
        /// Stores <paramref name="command"/> in the unpublished commands collection.
        /// </summary>
        /// <param name="command">The command to publish.</param>
        /// <returns>The envelope for the <paramref name="command"/>.</returns>
        protected Envelope<ICommand> Publish(ICommand command)
        {
            Ensure.NotNull(command, "command");
            Envelope<ICommand> envelope = Envelope.Create(command);
            commands.Add(envelope);
            return envelope;
        }
    }
}
