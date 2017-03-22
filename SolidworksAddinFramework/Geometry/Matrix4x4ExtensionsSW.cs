using System;
using System.DoubleNumerics;
using SolidWorks.Interop.sldworks;
using Weingartner.WeinCad.Interfaces;
using Weingartner.WeinCad.Interfaces.Math;

namespace SolidworksAddinFramework.Geometry
{
    public static class Matrix4X4ExtensionsSw
    {


        public static MathTransform ToSwTransform(this Matrix4x4 m, IMathUtility math = null) => 
            (math ?? SwAddinBase.Active.Math).ToSwMatrix(m);
    }
}