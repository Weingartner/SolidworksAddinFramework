using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework
{
    public static class FeatureManagerExtensions
    {
        /// <summary>
        /// Insert a macro feature but with type safe arguments and optimized for C#. However don't call this
        /// directly. Subclass MacroFeatureBase and all the magic will be done for you.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="featMgr"></param>
        /// <param name="featureName"></param>
        /// <param name="parameterNames"></param>
        /// <param name="parameterTypes"></param>
        /// <param name="parameterValues"></param>
        /// <param name="editBodies"></param>
        /// <param name="opts"></param>
        public static void InsertMacroFeature<T>
            (this IFeatureManager featMgr, string featureName, string[] parameterNames, int[] parameterTypes, string[] parameterValues, IBody2[] editBodies, int opts)
        {
            object paramNamesAsObject = parameterNames;
            object paramTypesAsObject = parameterTypes;
            object paramValuesAsObject = parameterValues;

            IFeature macroFeature = featMgr.InsertMacroFeature3(featureName, typeof (T).FullName, null, (paramNamesAsObject),
                (paramTypesAsObject), (paramValuesAsObject), null, null, editBodies, null, opts);

            if (macroFeature == null)
            {
                MessageBox.Show("Unable to create feature");
            }
            Console.WriteLine(macroFeature);
        }


        /// <summary>
        /// Insert a macro feature using a MacroFeatureDataBase subclass to define the parameters.
        /// </summary>
        /// <typeparam name="TFeature"></typeparam>
        /// <typeparam name="TData"></typeparam>
        /// <param name="featMgr"></param>
        /// <param name="featureName"></param>
        /// <param name="editBodies"></param>
        /// <param name="opts"></param>
        /// <param name="data"></param>
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

        /// <summary>
        /// Format the object so that it can be stored in macro feature data values array as
        /// a string.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string ToFormattedString(object o)
        {
            var of = o as IFormattable;
            if (of == null)
            {
                if(o!=null)
                    return o.ToString();
                else
                {
                    return "";
                }
                
            }
            else
            {
                return of.ToString(null, CultureInfo.InvariantCulture);
            }
        }
    }

}