using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework
{
    public interface IInterpolatable<T>
        where T : IInterpolatable<T>
    {
        T Interpolate(T other, double blend);
        MathTransform Transform(IMathUtility math);

    }
}