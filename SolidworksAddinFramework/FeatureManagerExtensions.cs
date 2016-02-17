using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework
{
    public static class FeatureManagerExtensions
    {
        public static void InsertMacroFeature<T>(this IFeatureManager featMgr, string featureName, string[] names, int[] types, string[] values, IBody2[] editBodies, int opts)
        {
            object paramNames = names;
            object paramTypes = types;
            object paramValues = values;

            IFeature macroFeature = featMgr.InsertMacroFeature3(featureName, typeof (T).FullName, null, (paramNames),
                (paramTypes), (paramValues), null, null, editBodies, null, opts);
            Console.WriteLine(macroFeature);
        }


        public static void InsertMacroFeature<TFeature,TData>(IFeatureManager featMgr, string featureName,
            IEnumerable<IBody2> editBodies, int opts, TData data)
            where TData : MacroFeatureDataBase, new()
        {
            InsertMacroFeature<TFeature>(featMgr, featureName,
                data.Names,
                data.Types,
                data.Values.Select(ToFormattedString).ToArray(),
                editBodies?.ToArray() ?? new IBody2[] {},
                opts);
        }

        public static string ToFormattedString(object o)
        {
            var of = o as IFormattable;
            if (of == null)
                return o.ToString();
            else
            {
                return of.ToString(null, CultureInfo.InvariantCulture);
            }
        }
    }

}