using System;
using System.Windows.Forms;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidworksAddinFramework
{
    public static class FeatureManagerExtensions
    {
        private static Feature InsertMacroFeature<T>(this IFeatureManager featMgr, string featureName, string[] parameterNames, int[] parameterTypes, string[] parameterValues, swMacroFeatureOptions_e opts, IBody2[] editBodies)
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
                editBodies,
                null,
                (int) opts);

            if (macroFeature == null)
            {
#if DEBUG
                var message = GetFeatureInsertError(typeof (T), parameterNames, parameterTypes, parameterValues);
#else
                var message = "";
#endif
                MessageBox.Show($"Unable to create feature {typeof(T).FullName}. {message}");
            }
            return macroFeature;
        }

        private static object GetFeatureInsertError(Type type, string[] parameterNames, int[] parameterTypes, string[] parameterValues)
        {
            if (type.GetConstructor(Type.EmptyTypes) == null)
            {
                return $"Type {type.FullName} doesn't have a public, parameterless constructor.";
            }
            if (parameterNames.Length != parameterTypes.Length || parameterNames.Length != parameterValues.Length)
            {
                var parameterNamesString = string.Join(", ", parameterNames);
                var parameterTypesString = string.Join(", ", parameterTypes);
                var parameterValuesString = string.Join(", ", parameterValues);
                return $"`parameterNames`() {parameterNamesString}, `parameterTypes`({parameterTypesString}) and " +
                       $"`parameterValues`({parameterValuesString}) don't have the same length.";
            }
            return $"Unknown reason. Ensure {type.FullName} lives in the addin dll. If you can fix it and there was another reason, please add a hint in {typeof(FeatureManagerExtensions).FullName}::{nameof(GetFeatureInsertError)}.";
        }

        public static Feature InsertMacroFeature<TFeature>(this IFeatureManager featMgr, string featureName, swMacroFeatureOptions_e opts, object data, IBody2[] editBodies)
        {
            return featMgr.InsertMacroFeature<TFeature>(featureName,
                MacroFeatureDataExtensions.GetFeatureDataNames(),
                MacroFeatureDataExtensions.GetFeatureDataTypes(),
                MacroFeatureDataExtensions.GetFeatureDataValues(data),
                opts,
                editBodies);
        }
    }

}