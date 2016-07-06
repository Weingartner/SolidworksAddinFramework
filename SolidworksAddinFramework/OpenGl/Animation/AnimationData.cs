using System.Numerics;

namespace SolidworksAddinFramework.OpenGl.Animation
{
    public class AnimationData
    {
        public IAnimationSection Section { get; }
        public Matrix4x4 Transform { get; }

        public AnimationData(IAnimationSection section, Matrix4x4 transform)
        {
            Section = section;
            Transform = transform;
        }
    }
}