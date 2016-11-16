using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace Neptuo.Linq.Expressions
{
    public class TypeHelper
    {
        public static string PropertyName<T>(Expression<Func<T, object>> propertyGetter)
        {
            return (propertyGetter.Body as MemberExpression ?? ((UnaryExpression)propertyGetter.Body).Operand as MemberExpression).Member.Name;
        }

        public static string PropertyName<T, TResult>(Expression<Func<T, TResult>> propertyGetter)
        {
            return (propertyGetter.Body as MemberExpression ?? ((UnaryExpression)propertyGetter.Body).Operand as MemberExpression).Member.Name;
        }

        public static string MethodName<T, TResult>(Expression<Func<T, Func<TResult>>> propertyGetter)
        {
            return (((((UnaryExpression)propertyGetter.Body).Operand as MethodCallExpression).Object as ConstantExpression).Value as MethodInfo).Name;
        }

        public static string MethodName<T, TParam1, TResult>(Expression<Func<T, Func<TParam1, TResult>>> propertyGetter)
        {
            return (((((UnaryExpression)propertyGetter.Body).Operand as MethodCallExpression).Object as ConstantExpression).Value as MethodInfo).Name;
        }

        public static string MethodName<T, TParam1, TParam2, TResult>(Expression<Func<T, System.Func<TParam1, TParam2, TResult>>> propertyGetter)
        {
            return (((((UnaryExpression)propertyGetter.Body).Operand as MethodCallExpression).Object as ConstantExpression).Value as MethodInfo).Name;
        }

        public static string MethodName<T, TParam1, TParam2, TParam3, TResult>(Expression<Func<T, System.Func<TParam1, TParam2, TParam3, TResult>>> propertyGetter)
        {
            return (((((UnaryExpression)propertyGetter.Body).Operand as MethodCallExpression).Object as ConstantExpression).Value as MethodInfo).Name;
        }

        public static string MethodName<T, TParam1, TParam2, TParam3, TParam4, TResult>(Expression<Func<T, System.Func<TParam1, TParam2, TParam3, TParam4, TResult>>> propertyGetter)
        {
            return (((((UnaryExpression)propertyGetter.Body).Operand as MethodCallExpression).Object as ConstantExpression).Value as MethodInfo).Name;
        }

        public static string MethodName<T>(Expression<Func<T, Action>> propertyGetter)
        {
            return (((((UnaryExpression)propertyGetter.Body).Operand as MethodCallExpression).Object as ConstantExpression).Value as MethodInfo).Name;
        }

        public static string MethodName<T, TParam1>(Expression<Func<T, Action<TParam1>>> propertyGetter)
        {
            return (((((UnaryExpression)propertyGetter.Body).Operand as MethodCallExpression).Object as ConstantExpression).Value as MethodInfo).Name;
        }

        public static string MethodName<T, TParam1, TParam2>(Expression<Func<T, Action<TParam1, TParam2>>> propertyGetter)
        {
            return (((((UnaryExpression)propertyGetter.Body).Operand as MethodCallExpression).Object as ConstantExpression).Value as MethodInfo).Name;
        }

        public static string MethodName<T, TParam1, TParam2, TParam3>(Expression<Func<T, Action<TParam1, TParam2, TParam3>>> propertyGetter)
        {
            return (((((UnaryExpression)propertyGetter.Body).Operand as MethodCallExpression).Object as ConstantExpression).Value as MethodInfo).Name;
        }

        public static string MethodName<T, TParam1, TParam2, TParam3, TParam4>(Expression<Func<T, Action<TParam1, TParam2, TParam3, TParam4>>> propertyGetter)
        {
            return (((((UnaryExpression)propertyGetter.Body).Operand as MethodCallExpression).Object as ConstantExpression).Value as MethodInfo).Name;
        }
    }
}