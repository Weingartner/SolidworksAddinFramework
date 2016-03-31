using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SolidworksAddinFramework.Reflection
{
    public static class ExpressionExtensions
    {
        public static Action<TProperty> GetSetter<T, TProperty>(this Expression<Func<T, TProperty>> expression, T t)
        {
            var setter = BuildSet(expression);
            return v => setter(t, v);
        }

        static Action<T, TValue> BuildSet<T, TValue>(Expression<Func<T, TValue>> selector)
        {
            var props = ReactiveUI.Reflection.ExpressionToPropertyNames(selector.Body);
            return BuildSet<T,TValue>(props);
        }

        private static Action<T, TValue> BuildSet<T, TValue>(string property)
        {
            string[] props = property.Split('.');
            Type type = typeof(T);
            ParameterExpression arg = Expression.Parameter(type, "x");
            ParameterExpression valArg = Expression.Parameter(typeof(TValue), "val");
            Expression expr = arg;
            foreach (string prop in props.Take(props.Length - 1))
            {
                // use reflection (not ComponentModel) to mirror LINQ 
                PropertyInfo pi = type.GetProperty(prop);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            // final property set...
            PropertyInfo finalProp = type.GetProperty(props.Last());
            MethodInfo setter = finalProp.GetSetMethod();
            expr = Expression.Call(expr, setter, valArg);
            return Expression.Lambda<Action<T, TValue>>(expr, arg, valArg).Compile();

        }

        public static ISelectorProxy<TProperty> GetProxy<T, TProperty>(this Expression<Func<T, TProperty>> expression, T t)
        {
            return new SelectorProxy<T,TProperty>(expression, t);
        }

        public interface ISelectorProxy<U>
        {
            U Value { get; set; }
        };
        public class SelectorProxy<T,U> : ISelectorProxy<U>
        {
            private readonly T _T;
            public Action<U> Set { get; }
            public Func<T, U> Get { get; }

            public SelectorProxy(Expression<Func<T, U>> selector, T t )
            {
                _T = t;
                Set = selector.GetSetter(t);
                Get = selector.Compile();
            }

            public U Value
            {
                get
                {
                    return Get(_T);
                }
                set
                {
                    Set(value);
                }
            }
        }

    }
}
