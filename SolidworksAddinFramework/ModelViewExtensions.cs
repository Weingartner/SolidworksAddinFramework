using System;
using System.Collections.Generic;
using System.Text;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework
{
    public static class ModelViewExtensions
    {
        /// <summary>
        /// Find the vector in model space from the point to the viewers eye.
        /// </summary>
        /// <param name="modelView"></param>
        /// <param name="mathUtility"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        private static MathVector ViewVector(IModelView modelView, IMathUtility mathUtility, IMathPoint p)
        {
            var world2screen = modelView.Transform;
            var pScreen = (MathPoint) p.MultiplyTransform(world2screen);
            var vv = (IMathVector) mathUtility.CreateVector(new[] {0.0, 0, 1});
            var pScreenUp = (MathPoint) pScreen.AddVector(vv);
            var pWorldDelta = (MathPoint) pScreenUp.MultiplyTransform((MathTransform) world2screen.Inverse());
            var viewVector = (MathVector) p.Subtract(pWorldDelta);
            return viewVector;
        }
    }
}
