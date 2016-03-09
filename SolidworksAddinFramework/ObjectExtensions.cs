using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolidworksAddinFramework
{
    public static class ObjectExtensions
    {
        public static T[] CastArray<T>(this object o)
        {
            {
                var objects = o as T[];
                if (objects != null)
                    return objects;
            }

            {
                var objects = o as IEnumerable<object>;
                if (objects != null)
                    return objects.Cast<T>().ToArray();
            }

            throw new InvalidCastException($"Cannot cast {o.GetType().Name} to {typeof(T).Name} ");
        }
    }
}
