using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidworksAddinFramework
{
    public static class ComponentExtensions
    {
        public struct ExternalReferenceInfo
        {
            /// <summary>Path and name of the part or assembly</summary>
            public string ModelPathName;
            /// <summary>Path and name of the referenced part or component</summary>
            public string ComponentPathName;
            /// <summary>In-context items (sketches, features, and so on)</summary>
            public string Feature;
            /// <summary>Data used to create the items (converted edge or face, converted or offset sketch entity, body, and so on)</summary>
            public string DataType;
            /// <summary>Status of external reference</summary>
            public swExternalReferenceStatus_e Status;
            /// <summary>Actual item being used and the name of the document that contains the item</summary>
            public string RefEntity;
            /// <summary>Name of the component in which the affected feature exists; this information is only displayed when one or more RefEntity is in a different component in an assembly and does not apply to derived parts</summary>
            public string FeatCom;
            /// <summary>Configuration option</summary>
            public swExternalFileReferencesConfig_e ConfigOption;
            /// <summary>Name of the configuration when ConfigOption is swExternalFileReferencesNamedConfig</summary>
            public string ConfigName;

            /*
             
            Sample values

            ModelPathName           = C:\Users\Tim\AppData\Local\Temp\tmpE9FF.SLDASM
            ComponentPathName       = C:\temp\june test\350107.sldprt
            Feature                 = Sketch2  of  Surface-Extrude1
            DataType                = Convert Edge
            Status                  = swExternalReferenceInContext
            RefEntity               = Edge of 350107<1>
            FeatCom                 = Part1^tmpE9FF<1>
            ConfigOption            = swExternalFileReferencesConfigNone
            ConfigName              = 
             
            */
        }
        /// <summary>
        /// Setting up to call and then processing the results from ListExternalFileReferences2 
        /// requires many lines of code when directly calling the API.  
        /// This helper method shortens the amount of code required and makes it more readable.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ExternalReferenceInfo> ListExternalFileReferences2(this IComponent2 comp)
        {
            List<ExternalReferenceInfo> results = new List<ExternalReferenceInfo>();
            object vModelPathName = null;
            object vComponentPathName = null;
            object vFeature = null;
            object vDataType = null;
            object vStatus = null;
            object vRefEntity = null;
            object vFeatCom = null;
            int configOption;
            string configName;
            comp.ListExternalFileReferences2(out vModelPathName, out vComponentPathName, out vFeature, out vDataType, out vStatus, out vRefEntity, out vFeatCom, out configOption, out configName);
            if (vModelPathName != null && vComponentPathName != null && vFeature != null && vDataType != null && vStatus != null && vRefEntity != null && vFeatCom != null)
            {
                var modelPathNames = vModelPathName.CastArray<string>();
                var componentPathNames = vComponentPathName.CastArray<string>();
                var features = vFeature.CastArray<string>();
                var dataTypes = vDataType.CastArray<string>();
                var statuses = vStatus.CastArray<swExternalReferenceStatus_e>();
                var refEntities = vRefEntity.CastArray<string>();
                var featComs = vFeatCom.CastArray<string>();
                for (int i = 0; i < modelPathNames.Count(); i++)
                {
                    results.Add(new ExternalReferenceInfo { ModelPathName = modelPathNames[i], ComponentPathName = componentPathNames[i], Feature = features[i],
                        DataType = dataTypes[i], Status = statuses[i], RefEntity = refEntities[i], FeatCom = featComs[i],
                        ConfigOption = (swExternalFileReferencesConfig_e)configOption, ConfigName = configName });
                }
            }
            return results;
        }
    }
}
