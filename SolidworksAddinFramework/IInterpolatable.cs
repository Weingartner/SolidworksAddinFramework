using System.Numerics;

namespace SolidworksAddinFramework
{
    public interface IInterpolatable<T>
        where T : IInterpolatable<T>
    {
        T Interpolate(T other, double blend);
        Matrix4x4 Transform();

    }
}