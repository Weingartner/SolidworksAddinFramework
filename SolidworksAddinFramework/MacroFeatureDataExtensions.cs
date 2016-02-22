using System;
using System.Collections.Generic;
using System.Linq;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidworksAddinFramework
{
    public static class MacroFeatureDataExtensions
    {
        public static IEnumerable<object> GetSelectedObjects(this IMacroFeatureData selMgr, Func<swSelectType_e, int, bool> filter)
        {
            {
                object objects;
                object objectTypes;
                object marks;
                object drViews;
                object componentXForms;
                selMgr.GetSelections3(out objects, out objectTypes, out marks, out drViews, out componentXForms);

                var objectsArray = (object[])objects;
                var typesArray = (swSelectType_e[])objectTypes;
                var marksArray = (int[])marks;

                var i = 0;
                foreach (var item in objectsArray)
                {
                    if(filter(typesArray[i], marksArray[i]))
                    {
                        yield return item;
                    }
                    i++;
                }

            }
        }

    }
}