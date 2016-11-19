using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Models.Keys
{
    /// <summary>
    /// Base implementation of <see cref="IKey"/>.
    /// Solves equality, hash codes, comparing to other keys, atc.
    /// </summary>
    public abstract class KeyBase : IKey
    {
        /// <summary>
        /// Constant for hash code computing of the type.
        /// </summary>
        private const int hashPrimeNumber = 216613626;

        /// <summary>
        /// Constant for hash code computing of the hash code value provided by derivered class.
        /// </summary>
        private const int hashPrimeNumberField = 16777619;

        public string Type { get; private set; }

        public bool IsEmpty { get; private set; }

        /// <summary>
        /// Creates key instance with flag whether is empty or not..
        /// </summary>
        /// <param name="type">Identifier of the domain model type.</param>
        /// <param name="isEmpty">Whether this key is empty.</param>
        protected KeyBase(string type, bool isEmpty)
        {
            Ensure.NotNullOrEmpty(type, "type");
            Type = type;
            IsEmpty = isEmpty;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IKey);
        }

        public bool Equals(IKey other)
        {
            KeyBase key = other as KeyBase;
            if (key == null)
                return false;

            if (IsEmpty != key.IsEmpty)
                return false;

            if (Type != key.Type)
                return false;

            return Equals(key);
        }

        /// <summary>
        /// Should compare this key value to value of <paramref name="other"/> and returns its values are equal.
        /// </summary>
        /// <param name="other">The other key to compare its value.</param>
        /// <returns></returns>
        protected abstract bool Equals(KeyBase other);

        public int CompareTo(object obj)
        {
            KeyBase key = obj as KeyBase;
            if (key == null)
                return 1;

            int typeCompare = Type.CompareTo(key.Type);
            if (typeCompare == 0)
                return CompareValueTo(key);

            return typeCompare;
        }

        /// <summary>
        /// Should compare value of the <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The other key to compare its value to.</param>
        /// <rereturns><see cref="IComparable.CompareTo"/>.</rereturns>
        protected abstract int CompareValueTo(KeyBase other);


        public override int GetHashCode()
        {
            //TODO: Do NOT export this method?
            //unchecked // Overflow is fine, just wrap ...
            //{
                int hash = hashPrimeNumber;

                hash = hash * hashPrimeNumberField ^ Type.GetHashCode();
                hash = !IsEmpty
                    ? hash * hashPrimeNumberField ^ GetValueHashCode()
                    : hash * hashPrimeNumberField ^ -1;

                return hash;
            //}
        }

        /// <summary>
        /// Should returns hash code for this key value.
        /// </summary>
        /// <returns>Hash code for this key value.</returns>
        protected abstract int GetValueHashCode();

        /// <summary>
        /// Tries to cast <paramref name="other"/> to <typeparamref name="T"/> or convert it using <see cref="Converts"/>.
        /// </summary>
        /// <typeparam name="T">The type of the target key.</typeparam>
        /// <param name="other">The key to convert.</param>
        /// <param name="key">The converter key.</param>
        /// <returns><c>true</c> if conversion was successful and <paramref name="key"/> is set; otherwise <c>false</c> and <paramref name="key"/> is <c>null</c>.</returns>
        protected bool TryConvert<T>(KeyBase other, out T key)
            where T : KeyBase
        {
            key = other as T;
            if (other == null && !Converts.Try<IKey, T>(other, out key))
                return false;

            return true;
        }

        public override string ToString()
        {
            string value = "";
            if (IsEmpty)
                value = "empty";
            else
                value = ToStringValue();

            return String.Format("{0}({1}, {2})", GetType().Name, Type, value);
        }

        protected abstract string ToStringValue();
    }
}
