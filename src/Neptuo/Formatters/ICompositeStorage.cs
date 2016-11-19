using Neptuo.Collections.Specialized;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters
{
    /// <summary>
    /// The key-value storage of composite values.
    /// </summary>
    public interface ICompositeStorage : IKeyValueCollection
    {
        /// <summary>
        /// Truncates current state and loads content from <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The serialized storage.</param>
        void Load(Stream input);

        /// <summary>
        /// Truncates current state and loads content from <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The serialized storage.</param>
        /// <returns>The continuation task.</returns>
        Task LoadAsync(Stream input);

        /// <summary>
        /// Stores current state to the <paramref name="output"/>.
        /// </summary>
        /// <param name="output">The stream to serialize to.</param>
        void Store(Stream output);

        /// <summary>
        /// Stores current state to the <paramref name="output"/>.
        /// </summary>
        /// <param name="output">The stream to serialize to.</param>
        /// <returns>The continuation task.</returns>
        Task StoreAsync(Stream output);

        /// <summary>
        /// Stores <paramref name="value"/> with <paramref name="key"/>.
        /// Returns self (for fluency).
        /// </summary>
        /// <param name="key">The key to associate <paramref name="value"/> with.</param>
        /// <param name="value">The value to store.</param>
        /// <returns>Self (for fluency).</returns>
        new ICompositeStorage Add(string key, object value);

        /// <summary>
        /// Creates sub-storage with <paramref name="key"/>.
        /// Returns child storage.
        /// </summary>
        /// <param name="key">The key to associate new storage with.</param>
        /// <returns>The child storage.</returns>
        ICompositeStorage Add(string key);

        /// <summary>
        /// Tries to get child storage associated with <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key to retrieve a value of.</param>
        /// <param name="storage">The associated child storage.</param>
        /// <returns><c>true</c> if <paramref name="key"/> was found; <c>false</c> otherwise.</returns>
        bool TryGet(string key, out ICompositeStorage storage);
    }
}
