using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework
{
    public static class SolidworksSelectionDataExtensions
    {
        public static IEnumerable<object> GetObjects(this SelectionData selectionData, IModelDoc2 doc)
        {
            return selectionData.ObjectIds
                                .Select(objectId => doc.GetObjectFromPersistReference(objectId.Data));
        }
        /// <summary>
        /// Gets an evaluator for the selected object. We return Func because if you return the solidworks
        /// object itself and store it you get burned by solidworks rebuilds when the object is invalidated.
        /// Only evaluate the function when you actually need the solidworks object. If the return value
        /// is None then it means that there is nothing selected.
        /// </summary>
        /// <param name="selectionData"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static Option<Func<object>> GetSingleObject(this SelectionData selectionData, IModelDoc2 doc) => 
            selectionData.IsEmpty
                ? Prelude.None
                : Prelude.Some(Prelude.fun(()=> selectionData.GetObjects(doc).First()));

        public static IEnumerable<T> GetObjects<T>(this SelectionData selectionData, IModelDoc2 doc)
        {
            return from o in selectionData.GetObjects(doc)
                   select o.DirectCast<T>();
        }
        public static Option<Func<T>> GetSingleObject<T>(this SelectionData selectionData, IModelDoc2 doc)
        {
            return from o in GetSingleObject(selectionData, doc)
                   select Prelude.fun(()=> o().DirectCast<T>());
        }
        
    }
}