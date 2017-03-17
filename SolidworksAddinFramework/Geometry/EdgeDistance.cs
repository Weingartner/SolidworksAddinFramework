using Weingartner.WeinCad.Interfaces;
using Weingartner.WeinCad.Interfaces.Math;

namespace SolidworksAddinFramework.Geometry
{
    public struct EdgeDistance
    {
        public Edge3 Edge { get; }
        public double Distance { get; }


        public EdgeDistance(Edge3 edge, double distance)
        {
            Edge = edge;
            Distance = distance;
        }
    }
}