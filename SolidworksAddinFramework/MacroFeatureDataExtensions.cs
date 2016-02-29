using System;
using System.Collections.Generic;
using System.Linq;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidworksAddinFramework
{
    public static class MacroFeatureDataExtensions
    {
        /// <summary>
        /// Return selected objects filtered by type and mark.
        /// </summary>
        /// <param name="selMgr"></param>
        /// <param name="filter">(type,mark)=>bool</param>
        /// <returns></returns>
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

        /// <summary>
        /// Adds tracking ids to a body faces and edges. This is not the best way to 
        /// do this in a repeatable way but works quickly.
        /// </summary>
        /// <param name="macroFeatureData"></param>
        /// <param name="body"></param>
        public static void AddIdsToBody(this IMacroFeatureData macroFeatureData, IBody2 body)
        {
            if (body == null) throw new ArgumentNullException(nameof(body));
            {
                object FacesNeedId;
                object EdgesNeedId;
                macroFeatureData.GetEntitiesNeedUserId((object)body, out FacesNeedId, out EdgesNeedId);
                var edgesNeedId = (object[]) EdgesNeedId;
                var facesNeedId = (object[]) FacesNeedId;
                int i = 0;
                var empty = new object[] {}; 
                foreach (var edge in edgesNeedId ?? empty)
                {
                    macroFeatureData.SetEdgeUserId((Edge) edge, i, 0);
                    i++;
                }
                foreach (var face in facesNeedId ?? empty)
                {
                    macroFeatureData.SetFaceUserId((Face2) face, i, 0);
                    i++;
                }
            }
        }
    }
}