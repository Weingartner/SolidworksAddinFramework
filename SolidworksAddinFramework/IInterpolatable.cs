using System.Numerics;

namespace SolidworksAddinFramework
{
    public interface IInterpolatable<T>
        where T : IInterpolatable<T>
    {
        T Interpolate(T other, double blend);
        /// <summary>
        /// Notify to the object that the animation is at this point
        /// </summary>
        /// <param name="other"></param>
        /// <param name="blend"></param>
        void Notify(T other, double blend);
        Matrix4x4 Transform();

    }
}