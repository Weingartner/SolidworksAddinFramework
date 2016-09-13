using System.DoubleNumerics;

namespace SolidworksAddinFramework
{
    public interface IInterpolatable<T>
        where T : IInterpolatable<T>
    {
        T Interpolate(T other, double blend, bool checkRange=true);
        Matrix4x4 Transform();

    }
}