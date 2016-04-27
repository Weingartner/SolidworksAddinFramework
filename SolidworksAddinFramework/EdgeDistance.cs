using SolidworksAddinFramework.Geometry;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework
{
    public struct EdgeDistance
    {
        public Edge3 Edge { get; }
        public double Distance { get; }


        public EdgeDistance(Edge3 edge, double distance)
        {
            this.Edge = edge;
            this.Distance = distance;
        }
    }
}