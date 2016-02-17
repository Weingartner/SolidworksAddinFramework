using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace SolidworksAddinFramework
{
    public class ComWangling
    {
        /// <summary>
        /// Given a COM object try to figure out it's interface type using a brute force
        /// search on all interfaces in the assembly associated with type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetComTypeFor<T>(object o)
        {
            return typeof (T).Assembly.GetTypes().Where(t => t.IsInterface)
                .Where(t =>
                {
                    try
                    {
                        Marshal.GetComInterfaceForObject(o, t);
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                });
        }

        public static DispatchWrapper[] ObjectArrayToDispatchWrapper(IEnumerable<object> objects)
        {
            return objects.Select(o => new DispatchWrapper(o)).ToArray();
        }
    }
}