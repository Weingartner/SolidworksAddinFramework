using System;
using System.Collections.Concurrent;

namespace SwCSharpAddinSpecHelper
{
    public class Pool<T>
    {
        private readonly Func<T> _Generator;
        private readonly ConcurrentBag<T> _Objects = new ConcurrentBag<T>();

        public Pool(Func<T> generator)
        {
            _Generator = generator;
        }

        public T Acquire()
        {
            T obj;
            if (!_Objects.TryTake(out obj))
            {
                obj = _Generator();
            }
            return obj;
        }

        public void Release(T obj)
        {
            _Objects.Add(obj);
        }

        public void ForEach(Action<T> action)
        {
            T obj;
            while (_Objects.TryTake(out obj))
            {
                action(obj);
            }
        }
    }
}