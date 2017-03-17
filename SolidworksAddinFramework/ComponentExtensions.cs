using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System.Collections.Generic;
using System.Linq;
using Weingartner.WeinCad.Interfaces;

namespace SolidworksAddinFramework
{
    public static class ComponentExtensions
    {
        public class ExternalReferenceInfo
        {
            public ExternalReferenceInfo(string modelPathName,string componentPathName,string feature,string dataType,swExternalReferenceStatus_e status,string refEntity,string featCom, swExternalFileReferencesConfig_e configOption,string configName)
            {
                this.ModelPathName = modelPathName;
                this.ComponentPathName = componentPathName;
                this.Feature = feature;
                this.DataType = dataType;
                this.Status = status;
                this.RefEntity = refEntity;
                this.FeatCom = featCom;
                this.ConfigOption = configOption;
                this.ConfigName = configName;
            }
            /// <summary>Path and name of the part or assembly</summary>
            public readonly string ModelPathName;
            /// <summary>Path and name of the referenced part or component</summary>
            public readonly string ComponentPathName;
            /// <summary>In-context items (sketches, features, and so on)</summary>
            public readonly string Feature;
            /// <summary>Data used to create the items (converted edge or face, converted or offset sketch entity, body, and so on)</summary>
            public readonly string DataType;
            /// <summary>Status of external reference</summary>
            public readonly swExternalReferenceStatus_e Status;
            /// <summary>Actual item being used and the name of the document that contains the item</summary>
            public readonly string RefEntity;
            /// <summary>Name of the component in which the affected feature exists; this information is only displayed when one or more RefEntity is in a different component in an assembly and does not apply to derived parts</summary>
            public readonly string FeatCom;
            /// <summary>Configuration option</summary>
            public readonly swExternalFileReferencesConfig_e ConfigOption;
            /// <summary>Name of the configuration when ConfigOption is swExternalFileReferencesNamedConfig</summary>
            public readonly string ConfigName;

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
        public static IEnumerable<ExternalReferenceInfo> ListExternalFileReferences(this IComponent2 comp)
        {
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
            var modelPathNames = vModelPathName.CastArray<string>();
            var componentPathNames = vComponentPathName.CastArray<string>();
            var features = vFeature.CastArray<string>();
            var dataTypes = vDataType.CastArray<string>();
            var statuses = vStatus.CastArray<swExternalReferenceStatus_e>();
            var refEntities = vRefEntity.CastArray<string>();
            var featComs = vFeatCom.CastArray<string>();
            for (int i = 0; i < modelPathNames.Count(); i++)
            {
                yield return new ExternalReferenceInfo(modelPathNames[i], componentPathNames[i], features[i],dataTypes[i], statuses[i], 
                    refEntities[i], featComs[i],(swExternalFileReferencesConfig_e)configOption, configName );
            }
        }
    }
}
