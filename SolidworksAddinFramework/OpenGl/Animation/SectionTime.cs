using System;

namespace SolidworksAddinFramework.OpenGl.Animation
{
    public class SectionTime
    {
        public IAnimationSection Section { get; }

        public DateTime EndTime { get; }

        public SectionTime(IAnimationSection section, DateTime endTime)
        {
            Section = section;
            EndTime = endTime;
        }
    }
}