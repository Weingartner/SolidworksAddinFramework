using System;
using System.Drawing;
using System.DoubleNumerics;
using SolidWorks.Interop.sldworks;
using Point = Weingartner.WeinCad.Interfaces.Drawing.Point;

namespace SolidworksAddinFramework.OpenGl
{
    public static class PointExtensions
    {
        public static IDisposable DisplayUndoable(this Vector3 p, IModelDoc2 doc, Color color, int size, int layer = 0)
        {
            return new Point(p, color, size).DisplayUndoable(doc, layer);
        }

    }
}