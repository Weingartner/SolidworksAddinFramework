using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidworksAddinFramework
{
    public static class FeatureManagerExtensions
    {
        private static void InsertMacroFeature<T>(this IFeatureManager featMgr, string featureName, string[] parameterNames, int[] parameterTypes, string[] parameterValues, swMacroFeatureOptions_e opts, IEnumerable<IBody2> editBodies)
        {
            var macroFeature = featMgr.InsertMacroFeature3(
                featureName,
                typeof (T).FullName,
                null,
                parameterNames,
                parameterTypes,
                parameterValues,
                null,
                null,
                editBodies.ToArray(),
                null,
                (int) opts);

            if (macroFeature == null)
            {
                MessageBox.Show("Unable to create feature");
            }
            Console.WriteLine(macroFeature);
        }

        public static void InsertMacroFeature<TFeature>(this IFeatureManager featMgr, string featureName, swMacroFeatureOptions_e opts, object data, IEnumerable<IBody2> editBodies)
        {
            featMgr.InsertMacroFeature<TFeature>(featureName,
                MacroFeatureDataExtensions.GetFeatureDataNames(),
                MacroFeatureDataExtensions.GetFeatureDataTypes(),
                MacroFeatureDataExtensions.GetFeatureDataValues(data),
                opts,
                editBodies);
        }
    }

}