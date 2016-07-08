namespace SolidworksAddinFramework.OpenGl.Animation
{
    public class AnimationData
    {
        public IAnimationSection Section { get; }

        public AnimationData(IAnimationSection section)
        {
            Section = section;
        }
    }
}