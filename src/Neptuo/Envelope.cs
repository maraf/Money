using Neptuo.Collections.Specialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo
{
    /// <summary>
    /// The wrapper around type-less body with metadata.
    /// </summary>
    public class Envelope
    {
        /// <summary>
        /// Creates new instance of <see cref="Envelope{T}"/> with body.
        /// </summary>
        /// <typeparam name="T">Type of the body.</typeparam>
        /// <param name="body">The body of the evelope.</param>
        /// <returns>New instance of <see cref="Envelope{T}"/> with <paramref name="body"/>.</returns>
        public static Envelope<T> Create<T>(T body)
        {
            return new Envelope<T>(body);
        }

        /// <summary>
        /// Creates new instance of <see cref="Envelope"/> with body.
        /// </summary>
        /// <param name="body">The body of the evelope.</param>
        /// <returns>New instance of <see cref="Envelope"/> with <paramref name="body"/>.</returns>
        public static Envelope Create(object body)
        {
            return new Envelope(body);
        }


        /// <summary>
        /// The body of the evelope.
        /// </summary>
        public object Body { get; private set; }

        /// <summary>
        /// The collection of metadata.
        /// </summary>
        public IKeyValueCollection Metadata { get; private set; }

        /// <summary>
        /// Creates new instance with the <paramref name="body"/>.
        /// </summary>
        /// <param name="body">The body of the evelope.</param>
        public Envelope(object body)
        {
            Ensure.NotNull(body, "body");
            Body = body;
            Metadata = new KeyValueCollection();
        }

        /// <summary>
        /// Creates new instance with the <paramref name="body"/> and the <paramref name="metadata"/>.
        /// </summary>
        /// <param name="body">The body of the evelope.</param>
        /// <param name="metadata">The collection of the metadata. Reference is used (instead of copying items).</param>
        public Envelope(object body, IKeyValueCollection metadata)
        {
            Ensure.NotNull(body, "body");
            Ensure.NotNull(metadata, "metadata");
            Body = body;
            Metadata = metadata;
        }
    }
    
    /// <summary>
    /// The wrapper around body of <typeparamref name="T"/> with metadata.
    /// </summary>
    /// <typeparam name="T">The type of envelope body.</typeparam>
    public class Envelope<T> : Envelope
    {
        /// <summary>
        /// The body of the evelope.
        /// </summary>
        public new T Body { get; private set; }

        /// <summary>
        /// Creates new instance with the <paramref name="body"/>.
        /// </summary>
        /// <param name="body">The body of the evelope.</param>
        public Envelope(T body)
            : base(body)
        {
            Ensure.NotNull(body, "body");
            Body = body;
        }

        /// <summary>
        /// Creates new instance with the <paramref name="body"/> and the <paramref name="metadata"/>.
        /// </summary>
        /// <param name="body">The body of the evelope.</param>
        /// <param name="metadata">The collection of the metadata. Reference is used (instead of copying items).</param>
        public Envelope(T body, IKeyValueCollection metadata)
            : base(body, metadata)
        {
            Ensure.NotNull(body, "body");
            Body = body;
        }
    }
}
