using System;
using System.Numerics;

namespace SolidworksAddinFramework
{
    public class NotifyingInterpolatable<T> : IInterpolatable<NotifyingInterpolatable<T>>
        where T : IInterpolatable<T>
    {
        private readonly T _Inner;
        private readonly IObserver<T> _NotifyTransformObserver;

        public NotifyingInterpolatable(T inner, IObserver<T> notifyTransformObserver)
        {
            _Inner = inner;
            _NotifyTransformObserver = notifyTransformObserver;
        }

        public NotifyingInterpolatable<T> Interpolate(NotifyingInterpolatable<T> other, double blend)
        {
            var otherInner = _Inner.Interpolate(other._Inner, blend);
            return new NotifyingInterpolatable<T>(otherInner, _NotifyTransformObserver);
        }

        public Matrix4x4 Transform()
        {
            _NotifyTransformObserver.OnNext(_Inner);
            return _Inner.Transform();
        }
    }

    public static class NotifyingInterpolatable
    {
        public static NotifyingInterpolatable<T> Create<T>(T inner, IObserver<T> notifyTransformObserver)
            where T : IInterpolatable<T>
        {
            return new NotifyingInterpolatable<T>(inner, notifyTransformObserver);
        }
    }
}