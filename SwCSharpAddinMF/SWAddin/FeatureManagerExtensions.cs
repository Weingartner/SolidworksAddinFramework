using System;
using System.Globalization;
using System.Linq;
using SolidWorks.Interop.sldworks;

namespace SwCSharpAddinMF.SWAddin
{
    public static class FeatureManagerExtensions
    {
        public static void InsertMacroFeature<T>(this IFeatureManager featMgr, string featureName, string[] names, int[] types, string[] values, IBody2 editBody, int opts)
        {
            object paramNames = names;
            object paramTypes = types;
            object paramValues = values;

            IFeature MacroFeature = featMgr.InsertMacroFeature3(featureName, typeof (T).FullName, null, (paramNames),
                (paramTypes), (paramValues), null, null, editBody, null, opts);
            Console.WriteLine(MacroFeature);
        }


        public static void InsertMacroFeature<T>(IFeatureManager featMgr, string featureName,
            IBody2 editBody, int opts, T data)
            where T : MacroFeatureDataBase, new()
        {
            featMgr.InsertMacroFeature<SampleMacroFeature>(featureName,
                data.Names,
                data.Types,
                data.Values.Select(ToFormattedString).ToArray(),
                editBody,
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