using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace SolidworksAddinFramework
{
    public static class ComWangling
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

        /// <summary>
        /// Wrap an enumerable of objects into an array of DispatchWrapper. Sometimes you need this,
        /// sometimes you don't. If symptoms persist see your doctor!
        /// </summary>
        /// <param name="objects"></param>
        /// <returns></returns>
        public static DispatchWrapper[] ObjectArrayToDispatchWrapper(IEnumerable<object> objects)
        {
            return objects.Select(o => new DispatchWrapper(o)).ToArray();
        }

        private static readonly Encoding Encoding = Encoding.UTF8;
        public static string ReadAllText(this IStream stream)
        {
            using (var managedStream = new MemoryStream())
            {
                var managedBuffer = new byte[1024];
                var bytesReadBuffer = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)));
                int bytesRead;

                do
                {
                    stream.Read(managedBuffer, managedBuffer.Length, bytesReadBuffer);
                    bytesRead = Marshal.ReadInt32(bytesReadBuffer);
                    managedStream.Write(managedBuffer, 0, bytesRead);
                }
                while (bytesRead > 0);
                return Encoding.GetString(managedStream.ToArray());
            }
        }

        public static void WriteAllText(this IStream stream, string text)
        {
            var bytes = text != null ? Encoding.GetBytes(text) : new byte[0];
            var bytesWrittenBuffer = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)));
            stream.Write(bytes, bytes.Length, bytesWrittenBuffer);
            var bytesWritten = Marshal.ReadInt32(bytesWrittenBuffer);
            if (bytesWritten != bytes.Length)
            {
                throw new IOException($"Tried to write {bytes.Length} bytes, but {bytesWritten} bytes were written.");
            }
        }
    }
}