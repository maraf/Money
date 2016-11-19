using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters
{
    /// <summary>
    /// The common extensions for <see cref="ISerializer"/> and <see cref="IDeserializer"/>.
    /// </summary>
    public static class _FormatterExtensions
    {
        /// <summary>
        /// Serializes <paramref name="input"/> and returns string representation encoded in <see cref="Encoding.UTF8"/>.
        /// </summary>
        /// <param name="serializer">The serializer to use.</param>
        /// <param name="input">The input object to serialize.</param>
        /// <returns>The serialized <paramref name="input"/>.</returns>
        /// <exception cref="SerializationFailedException">When serialization was not sucessful.</exception>
        public static string Serialize(this ISerializer serializer, object input)
        {
            Ensure.NotNull(serializer, "serializer");
            Ensure.NotNull(input, "model");

            Type inputType = input.GetType();
            using (MemoryStream stream = new MemoryStream())
            {
                bool result = serializer.TrySerialize(input, new DefaultSerializerContext(inputType, stream));
                if (result)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    return Encoding.UTF8.GetString(stream.ToArray());
                }
            }

            throw new SerializationFailedException(inputType);
        }

        /// <summary>
        /// Serializes <paramref name="input"/> and returns string representation encoded in <see cref="Encoding.UTF8"/>.
        /// </summary>
        /// <param name="serializer">The serializer to use.</param>
        /// <param name="input">The input object to serialize.</param>
        /// <returns>The serialized <paramref name="input"/>.</returns>
        /// <exception cref="SerializationFailedException">When serialization was not sucessful.</exception>
        public static async Task<string> SerializeAsync(this ISerializer serializer, object input)
        {
            Ensure.NotNull(serializer, "serializer");
            Ensure.NotNull(input, "model");

            Type inputType = input.GetType();
            using (MemoryStream stream = new MemoryStream())
            {
                bool result = await serializer.TrySerializeAsync(input, new DefaultSerializerContext(inputType, stream));
                if (result)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    return Encoding.UTF8.GetString(stream.ToArray());
                }
            }

            throw new SerializationFailedException(inputType);
        }

        /// <summary>
        /// Deserializes <paramref name="input"/> to the object of the type <paramref name="outputType"/> and returns it.
        /// </summary>
        /// <param name="deserializer">The deserializer to use.</param>
        /// <param name="outputType">The type of the object to create.</param>
        /// <param name="input">The serialized input.</param>
        /// <returns>The deserialized object of the type <paramref name="outputType"/>.</returns>
        public static object Deserialize(this IDeserializer deserializer, Type outputType, string input)
        {
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(input)))
            {
                DefaultDeserializerContext context = new DefaultDeserializerContext(outputType);
                bool result = deserializer.TryDeserialize(stream, context);
                if (result)
                    return context.Output;
            }

            throw new DeserializationFailedException(outputType, input);
        }

        /// <summary>
        /// Deserializes <paramref name="input"/> to the object of the type <typeparamref name="T"/> and returns it.
        /// </summary>
        /// <typeparam name="T">The type of the object to create.</typeparam>
        /// <param name="deserializer">The deserializer to use.</param>
        /// <param name="input">The serialized input.</param>
        /// <returns>The deserialized object of the type <paramref name="outputType"/>.</returns>
        public static T Deserialize<T>(this IDeserializer deserializer, string input)
        {
            return (T)Deserialize(deserializer, typeof(T), input);
        }

        /// <summary>
        /// Deserializes <paramref name="input"/> to the object of the type <paramref name="outputType"/> and returns it.
        /// </summary>
        /// <param name="deserializer">The deserializer to use.</param>
        /// <param name="outputType">The type of the object to create.</param>
        /// <param name="input">The serialized input.</param>
        /// <returns>The deserialized object of the type <paramref name="outputType"/>.</returns>
        public static async Task<object> DeserializeAsync(this IDeserializer deserializer, Type outputType, string input)
        {
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(input)))
            {
                DefaultDeserializerContext context = new DefaultDeserializerContext(outputType);
                bool result = await deserializer.TryDeserializeAsync(stream, context);
                if (result)
                    return context.Output;
            }

            throw new DeserializationFailedException(outputType, input);
        }

        /// <summary>
        /// Deserializes <paramref name="input"/> to the object of the type <typeparamref name="T"/> and returns it.
        /// </summary>
        /// <typeparam name="T">The type of the object to create.</typeparam>
        /// <param name="deserializer">The deserializer to use.</param>
        /// <param name="input">The serialized input.</param>
        /// <returns>The deserialized object of the type <paramref name="outputType"/>.</returns>
        public static async Task<T> DeserializeAsync<T>(this IDeserializer deserializer, string input)
        {
            return (T)(await DeserializeAsync(deserializer, typeof(T), input));
        }
    }
}
