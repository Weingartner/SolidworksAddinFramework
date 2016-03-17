using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolidworksAddinFramework
{
    public static class ObjectExtensions
    {
        public static T[] CastArray<T>(this object o)
        {
            if (o == null || o is System.DBNull)
                return new T[0];
            {
                var objects = o as T[];
                if (objects != null)
                    return objects;
            }

            {
                var objects = o as IEnumerable;
                if (objects != null)
                {
                    var objectarray = objects.Cast<object>().ToArray();
                    return Array.ConvertAll(objectarray, q =>
                    {
                        if (q is T)
                            return (T) q;
                        return (T) Convert.ChangeType(q, typeof (T));
                        
                    });

                }
            }

            throw new InvalidCastException($"Cannot cast {o.GetType().Name} to {typeof(T).Name} ");
        }
    }
}
