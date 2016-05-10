using System;
using System.Collections.Generic;
using System.Numerics;

namespace SolidworksAddinFramework.Geometry
{
    /// <summary>
    /// </summary>
    public struct Edge3
    {
        public readonly Vector3 A;
        public readonly Vector3 B;

        public double Length => Delta.Length();
        public double LengthSquared => Delta.LengthSquared();

        public Edge3(Vector3 a, Vector3 b)
        {
            A = a;
            B = b;
        }

        public Edge3(IList<Vector3> endPoints) : this(endPoints[0], endPoints[1])
        {
        }

        public Vector3 Delta => B - A;

        public Edge3 ApplyTransform(Matrix4x4 transform) => 
            new Edge3(Vector3.Transform(A, transform), Vector3.Transform(B, transform));

        /// <summary>
        /// Project the point onto the infinite edge
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector3 Project(Vector3 point)
        {
            var d = Delta;
            var t = Vector3.Dot(point - A, d)/d.LengthSquared();
            return Delta*t + A;
        }

        /// <summary>
        /// Find the closest point on the curve
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector3 ClosestPoint(Vector3 point)
        {
            var d = Delta;
            var t = Vector3.Dot(point - A, d)/d.LengthSquared();
            if (t > 0)
                return B;
            if (t < 0)
                return A;

            return Delta*t + A;
        }

        public Edge3 ShortestEdgeJoining(Edge3 other, double tol = 1e-9)
        {
            return ShortestConnectingEdge(this, other, tol);
        }
        public Edge3 ShortestEdgeJoining(PointDirection3 other, double tol = 1e-9)
        {
            return ShortestConnectingEdge(this, other, tol);
        }

        /// <summary>
        /// From http://geomalgorithms.com/a07-_distance.html
        /// </summary>
        /// <param name="e0"></param>
        /// <param name="e1"></param>
        /// <param name="tol"></param>
        /// <returns></returns>
        public static
            Edge3
            ShortestConnectingEdge
            (Edge3 e0, Edge3 e1, double tol = 1e-9)
        {
            var u = e0.B - e0.A;
            var v = e1.B - e1.A;
            var w = e0.A - e1.A;
            var a = Vector3.Dot(u, u);      // always >= 0
            var b = Vector3.Dot(u, v);
            var c = Vector3.Dot(v, v);         // always >= 0
            var d = Vector3.Dot(u, w);
            var e = Vector3.Dot(v, w);
            var det = a * c - b * b;
            float sN, sD = det;       // sc = sN / sD, default sD = D >= 0
            float tN, tD = det;       // tc = tN / tD, default tD = D >= 0

            // compute the line parameters of the two closest points
            if (det < tol)
            { 
                // the lines are almost parallel
                sN = 0.0f;         // force using point P0 on segment S1
                sD = 1.0f;         // to prevent possible division by 0.0 later
                tN = e;
                tD = c;
            }
            else
            {                 
                // get the closest points on the infinite lines
                sN = b * e - c * d;
                tN = a * e - b * d;
                if (sN < 0.0)
                {        
                    // sc < 0 => the s=0 edge is visible
                    sN = 0.0f;
                    tN = e;
                    tD = c;
                }
                else if (sN > sD)
                {  
                    // sc > 1  => the s=1 edge is visible
                    sN = sD;
                    tN = e + b;
                    tD = c;
                }
            }

            if (tN < 0.0)
            {            
                // tc < 0 => the t=0 edge is visible
                tN = 0.0f;
                // recompute sc for this edge
                if (-d < 0.0)
                    sN = 0.0f;
                else if (-d > a)
                    sN = sD;
                else
                {
                    sN = -d;
                    sD = a;
                }
            }
            else if (tN > tD)
            {      // tc > 1  => the t=1 edge is visible
                tN = tD;
                // recompute sc for this edge
                if ((-d + b) < 0.0)
                    sN = 0;
                else if ((-d + b) > a)
                    sN = sD;
                else
                {
                    sN = -d + b;
                    sD = a;
                }
            }
            // finally do the division to get sc and tc
            var sc = (float)(Math.Abs(sN) < tol ? 0.0 : sN / sD);
            var tc = (float)(Math.Abs(tN) < tol ? 0.0 : tN / tD);

            // get the difference of the two closest points
            var dP = w + (sc * u) - (tc * v);  // =  S1(sc) - e2(tc)

            var p1 = sc*u + e0.A;
            var p2 = tc*v + e1.A;

            return new Edge3(p1,p2);

        }
        //===================================================================



        /// <summary>
        /// From http://geomalgorithms.com/a07-_distance.html
        /// </summary>
        /// <param name="e1"></param>
        /// <param name="e2"></param>
        /// <param name="tol"></param>
        /// <returns></returns>
        public static
            Edge3
            ShortestConnectingEdge
            (Edge3 e1, PointDirection3 e2, double tol = 1e-9)
        {
            var u1 = e1.B - e1.A;
            var u2 = e2.Direction;
            var w = e1.A - e2.Point;
            var a = Vector3.Dot(u1, u1);      // always >= 0
            var b = Vector3.Dot(u1, u2);
            var c = Vector3.Dot(u2, u2);         // always >= 0
            var d = Vector3.Dot(u1, w);
            var e = Vector3.Dot(u2, w);
            var det = a * c - b * b;
            float t1, d1 = det;       // sc = sN / sD, default sD = D >= 0
            float t2, d2 = det;       // tc = tN / tD, default tD = D >= 0

            // compute the line parameters of the two closest points
            if (det < tol)
            { 
                // the lines are almost parallel
                t1 = 0.0f;         // force using point P0 on segment S1
                d1 = 1.0f;         // to prevent possible division by 0.0 later
                t2 = e;
                d2 = c;
            }
            else
            {                 
                // get the closest points on the infinite lines
                t1 = b * e - c * d;
                t2 = a * e - b * d;
                if (t1 < 0.0)
                {        
                    // sc < 0 => the s=0 edge is visible
                    t1 = 0.0f;
                    t2 = e;
                    d2 = c;
                }
                else if (t1 > d1)
                {  
                    // sc > 1  => the s=1 edge is visible
                    t1 = d1;
                    t2 = e + b;
                    d2 = c;
                }
            }

            // finally do the division to get sc and tc
            var c1 = (float)(Math.Abs(t1) < tol ? 0.0 : t1 / d1);
            var c2 = (float)(Math.Abs(t2) < tol ? 0.0 : t2 / d2);

            var p1 = c1*u1 + e1.A;
            var p2 = c2*u2 + e2.Point;

            return new Edge3(p1,p2);

        }




    }
}