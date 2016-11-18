using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Converters
{
    /// <summary>
    /// Any type to <see cref="String"/> converter with formatting support.
    /// </summary>
    public class ToStringConverter<TSource> : DefaultConverter<TSource, string>
    {
        private readonly string format;

        /// <summary>
        /// Creates new instance without formatter.
        /// </summary>
        public ToStringConverter()
        { }

        /// <summary>
        /// Creates new instance with <paramref name="format"/>.
        /// </summary>
        /// <param name="format">Format string, eg. yyyy-MM-dd for datetime.</param>
        public ToStringConverter(string format)
        {
            Ensure.NotNullOrEmpty(format, "format");
            this.format = format;
        }

        public override bool TryConvert(TSource sourceValue, out string targetValue)
        {
            string format = this.format == null ? "{0}" : "{0:" + this.format + "}";
            targetValue = String.Format(format, sourceValue);
            return true;
        }
    }

    /// <summary>
    /// Any type to <see cref="String"/> converter with formatting support.
    /// </summary>
    public class ToStringConverter : IConverter
    {
        private readonly string format;

        /// <summary>
        /// Creates new instance without formatter.
        /// </summary>
        public ToStringConverter()
        { }

        /// <summary>
        /// Creates new instance with <paramref name="format"/>.
        /// </summary>
        /// <param name="format">Format string, eg. yyyy-MM-dd for datetime.</param>
        public ToStringConverter(string format)
        {
            Ensure.NotNullOrEmpty(format, "format");
            this.format = format;
        }

        public bool TryConvert(Type sourceType, Type targetType, object sourceValue, out object targetValue)
        {
            if (targetType == typeof(string))
            {
                string format = this.format == null ? "{0}" : "{0:" + this.format + "}";
                targetValue = String.Format(format, sourceValue);
                return true;
            }

            targetValue = null;
            return false;
        }
    }
}
