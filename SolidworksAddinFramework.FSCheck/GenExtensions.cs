using System;
using System.Collections.Generic;
using System.DoubleNumerics;
using FsCheck;
using LanguageExt;
using SolidworksAddinFramework.Geometry;
using Weingartner.WeinCad.Interfaces;

namespace WeinCadSW.Spec.FsCheck
{
    public static class GenExtensions
    {

        public static Gen<Vector3> Vector3(this Gen<double> floatGen)
        {
            return floatGen
                .Three()
                .Select(t=>t.Map((a, b, c) => new Vector3(a, b, c)));
        }

        public static Gen<Triangle> Triangle(this Gen<Vector3> vectorGen)
        {
            return vectorGen
                .Three()
                .Select
                (t => t.Map((a, b, c) => new Triangle(a, b, c)));
        }

        /// <summary>
        /// Combine each item with every other item but not with themselves
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<Tuple<T, T>> XPaired<T>(this Lst<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = i; j < list.Count-1; j++)
                {
                    yield return Prelude.Tuple(list[i], list[j]);
                }
            }
        }
        
    }
}