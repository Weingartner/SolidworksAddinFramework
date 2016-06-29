using System;

namespace SolidworksAddinFramework
{
    public class ResourceDisposable<T> : IDisposable
    {
        public T Resource { get; }
        private readonly IDisposable _Disposable;

        public ResourceDisposable(T resource, params IDisposable[] disposables)
        {
            Resource = resource;
            _Disposable = disposables.ToCompositeDisposable();
        }

        public void Dispose()
        {
            _Disposable.Dispose();
        }
    }

    public static class ResourceDisposable
    {
        public static ResourceDisposable<T> Create<T>(T resource, params IDisposable[] disposables)
        {
            return new ResourceDisposable<T>(resource, disposables);
        }
    }
}
