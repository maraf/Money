using Neptuo.Collections.Specialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo
{
    /// <summary>
    /// The extensions for EventSourcing execution for <see cref="Envelope"/>.
    /// </summary>
    public static class _EnvelopeExecutionExtensions
    {
        #region ExecuteAt

        /// <summary>
        /// Sets 'ExecuteAt' to the metadata of the <paramref name="envelope"/>.
        /// </summary>
        /// <param name="envelope">The envelope which metadata to extend.</param>
        /// <param name="value">The vale of the 'ExecuteAt'.</param>
        /// <returns><paramref name="envelope"/>(for fluency).</returns>
        public static Envelope AddExecuteAt(this Envelope envelope, DateTime value)
        {
            Ensure.NotNull(envelope, "envelope");
            envelope.Metadata.Add("ExecuteAt", value);
            return envelope;
        }

        /// <summary>
        /// Tries to read 'ExecuteAt' from the metadata of the <paramref name="envelope"/>.
        /// </summary>
        /// <param name="envelope">The envelope to read the metadata from.</param>
        /// <param name="value">The value of the 'ExecuteAt'.</param>
        /// <returns><c>true</c> if 'ExecuteAt' can is contained in the <paramref name="envelope"/>; <c>false</c> otherwise.</returns>
        public static bool TryGetExecuteAt(this Envelope envelope, out DateTime value)
        {
            Ensure.NotNull(envelope, "envelope");
            return envelope.Metadata.TryGet("ExecuteAt", out value);
        }

        /// <summary>
        /// Reads 'ExecuteAt' from the metadata of the <paramref name="envelope"/> or throws <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <param name="envelope">The envelope to read the metadata from.</param>
        /// <returns>The value associated with the 'ExecuteAt' or throws <see cref="InvalidOperationException"/>.</returns>
        /// <exception cref="InvalidOperationException">When the 'ExecuteAt' is not contained in the metadata of the <paramref name="envelope"/>.</exception>
        public static DateTime GetExecuteAt(this Envelope envelope)
        {
            Ensure.NotNull(envelope, "envelope");
            return envelope.Metadata.Get<DateTime>("ExecuteAt");
        }

        #endregion

    }
}
