using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters.Metadata
{
    partial class ManualCompositeTypeProvider
    {
        public interface ITypeBuilder<T>
        {
            IVersionBuilder<T> AddVersion(int version);
            ITypeBuilder<TOther> Add<TOther>(Expression<Func<TOther, int>> versionGetter);
            ITypeBuilder<TOther> Add<TOther>(string typeName, Expression<Func<TOther, int>> versionGetter);
        }

        public interface IVersionBuilder<T>
        {
            IVersionBuilder<T, TValue> WithProperty<TValue>(Expression<Func<T, TValue>> getter);
        }

        public interface IVersionBuilder<T, TValue1>
        {
            IVersionBuilder<T, TValue1, TValue> WithProperty<TValue>(Expression<Func<T, TValue>> getter);
            ITypeBuilder<T> WithConstructor(Expression<Func<TValue1, T>> factory);
        }

        public interface IVersionBuilder<T, TValue1, TValue2>
        {
            IVersionBuilder<T, TValue1, TValue2, TValue> WithProperty<TValue>(Expression<Func<T, TValue>> getter);
            ITypeBuilder<T> WithConstructor(Expression<Func<TValue1, TValue2, T>> factory);
        }

        public interface IVersionBuilder<T, TValue1, TValue2, TValue3>
        {
            IVersionBuilder<T, TValue1, TValue2, TValue3, TValue> WithProperty<TValue>(Expression<Func<T, TValue>> getter);
            ITypeBuilder<T> WithConstructor(Expression<Func<TValue1, TValue2, TValue3, T>> factory);
        }

        public interface IVersionBuilder<T, TValue1, TValue2, TValue3, TValue4>
        {
            IVersionBuilder<T, TValue1, TValue2, TValue3, TValue4, TValue> WithProperty<TValue>(Expression<Func<T, TValue>> getter);
            ITypeBuilder<T> WithConstructor(Expression<Func<TValue1, TValue2, TValue3, TValue4, T>> factory);
        }

        public interface IVersionBuilder<T, TValue1, TValue2, TValue3, TValue4, TValue5>
        {
            IVersionBuilder<T, TValue1, TValue2, TValue3, TValue4, TValue5, TValue> WithProperty<TValue>(Expression<Func<T, TValue>> getter);
            ITypeBuilder<T> WithConstructor(Expression<Func<TValue1, TValue2, TValue3, TValue4, TValue5, T>> factory);
        }

        public interface IVersionBuilder<T, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>
        {
            ITypeBuilder<T> WithConstructor(Expression<Func<TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, T>> factory);
        }
    }
}
