using System;

namespace SolidworksAddinFramework
{
    /// <summary>
    /// Mark fields with this attribute to serialize them with the macro feature data.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MacroFeatureDataFieldAttribute : System.Attribute
    {
        
    }
}