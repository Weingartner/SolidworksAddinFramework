namespace SolidworksAddinFramework.Geometry
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