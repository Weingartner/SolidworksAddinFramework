using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;

namespace SwCSharpAddinMF.SWAddin
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MacroFeatureDataFieldAttribute : System.Attribute
    {
        
    }
    public enum StateEnum {  Insert, Edit }

    public class MacroFeatureDataBase
    {
        private int[] _Types;
        private string[] _Names;
        private object[] _Values;

        public MacroFeatureDataBase()
        {
            _Types = BindingProperties.Select(p =>
            {
                if (p.PropertyType == typeof (string))
                {
                    return swMacroFeatureParamType_e.swMacroFeatureParamTypeString;
                }
                else if (p.PropertyType == typeof (int))
                {
                    return swMacroFeatureParamType_e.swMacroFeatureParamTypeInteger;

                }
                else if (p.PropertyType == typeof (double))
                {
                    return swMacroFeatureParamType_e.swMacroFeatureParamTypeDouble;
                }
                else if (p.PropertyType == typeof (bool))
                {
                    return swMacroFeatureParamType_e.swMacroFeatureParamTypeInteger;
                }
                else
                {
                    throw new InvalidCastException($"Cannot bind the type {p.PropertyType.Name} ");
                }

            })
                .Select(v=>(int)v)
                .ToArray();
            _Names = BindingProperties.Select(p => p.Name).ToArray();
            _Values = BindingProperties.Select(p => p.GetValue(this, new object[] {})).ToArray();
        }

        private void UpdateValues()
        {
            var tmp = BindingProperties.Select(p => p.GetValue(this, new object[] {})).ToList();
            tmp.CopyTo(_Values);

        }

        private List<PropertyInfo> BindingProperties
        {
            get
            {
                return GetType().GetProperties()
                    .Where(p => p.GetCustomAttributes(typeof (MacroFeatureDataFieldAttribute), true).Length > 0)
                    .ToList();
            }
        }

        public string[] Names => _Names;

        public int[] Types => _Types;

        public object[] Values
        {
            get { UpdateValues();
                return _Values; }
        }

        public override string ToString()
        {
            return "{ " + string.Join(", ", BindingProperties
                .Select(prop => $"{prop.Name}: {prop.GetValue(this, new object[] {})}")) + " }"; 
        }

        public void WriteTo(IMacroFeatureData data)
        {
            foreach (var bindingProperty in BindingProperties)
            {
                if (bindingProperty.PropertyType == typeof (string))
                {
                    data.SetStringByName(bindingProperty.Name, (string) bindingProperty.GetValue(this,new object[] {}));
                }else if (bindingProperty.PropertyType == typeof (int))
                {
                    data.SetIntegerByName(bindingProperty.Name, (int) bindingProperty.GetValue(this,new object[] {}));
                    
                }else if (bindingProperty.PropertyType == typeof (double))
                {
                    data.SetDoubleByName(bindingProperty.Name, (double) bindingProperty.GetValue(this,new object[] {}));
                }
                else if (bindingProperty.PropertyType == typeof (bool))
                {
                    var value = (bool) bindingProperty.GetValue(this, new object[] {});
                    data.SetIntegerByName(bindingProperty.Name, value ? 1 : 0);
                }
                else
                {
                    throw new InvalidCastException($"Cannot bind the type {bindingProperty.PropertyType.Name} ");
                }
            }
        }

        public void ReadFrom(IMacroFeatureData data)
        {
            foreach (var bindingProperty in BindingProperties)
            {
                if (bindingProperty.PropertyType == typeof (string))
                {
                    string v;
                    data.GetStringByName(bindingProperty.Name, out v);
                    bindingProperty.SetValue(this,v,new object[] {});
                }else if (bindingProperty.PropertyType == typeof (int))
                {
                    int v;
                    data.GetIntegerByName(bindingProperty.Name, out v);
                    bindingProperty.SetValue(this,v,new object[] {});
                    
                }else if (bindingProperty.PropertyType == typeof (double))
                {
                    double v;
                    data.GetDoubleByName(bindingProperty.Name, out v);
                    bindingProperty.SetValue(this,v,new object[] {});
                }
                else if (bindingProperty.PropertyType == typeof (bool))
                {
                    int v;
                    data.GetIntegerByName(bindingProperty.Name, out v);
                    bindingProperty.SetValue(this,v==1,new object[] {});
                }
                else
                {
                    throw new InvalidCastException($"Cannot bind the type {bindingProperty.PropertyType.Name} ");
                }
            }
            
        }
        
    }

    public abstract class MacroFeatureBase<TMacroFeature,TData> : ISwComFeature
        where TData : MacroFeatureDataBase, new()
        where TMacroFeature : MacroFeatureBase<TMacroFeature, TData>
    {
        public IModelDoc2 ModelDoc { get; set; }

        public IFeature SwFeature { get; set; }

        public IMacroFeatureData SwFeatureData { get; set; }

        public ISldWorks SwApp { get; set; }
        public abstract TData Database { get; set; }

        public abstract string FeatureName { get; }

        public abstract swMacroFeatureOptions_e FeatureOptions { get; }

        public abstract IEnumerable<IBody2> EditBodies { get; }

        public ISelectionMgr SelectionMgr { get; set; }

        public object Edit(object app, object modelDoc, object feature)
        {
            Init(app, modelDoc, feature);
            LoadSelections();
            return Edit();
        }

        private void Init(object app, object modelDoc, object feature)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            if (modelDoc == null) throw new ArgumentNullException(nameof(modelDoc));
            // feature can be null

            SwApp = (ISldWorks) app;
            Database = new TData();
            if (feature != null)
            {
                SwFeature = (IFeature) feature;
                SwFeatureData = (IMacroFeatureData) SwFeature.GetDefinition();
                Database.ReadFrom(SwFeatureData);
            }
            ModelDoc = (IModelDoc2) modelDoc;
            SelectionMgr = (ISelectionMgr) ModelDoc.SelectionManager;
        }

        public StateEnum State => SwFeatureData == null ? StateEnum.Insert : StateEnum.Edit;

        public void Write()
        {
            SaveSelections(this);
            Database.WriteTo(SwFeatureData);
        }

        public void ModifyDefinition()
        {
            Write();
            SwFeature.ModifyDefinition(SwFeatureData, ModelDoc, null);
        }

        public void ReleaseSelectionAccess()
        {
            SwFeatureData?.ReleaseSelectionAccess();
        }

        public void InsertDefinition(string featureName, IEnumerable<IBody2> editBody, int opts)
        {

            FeatureManagerExtensions
                .InsertMacroFeature<TMacroFeature, TData>
                (ModelDoc.FeatureManager, featureName, editBody, opts, Database);
        }


        protected abstract object Edit();
        protected abstract object Security();
        protected abstract object Regenerate();

        public object Regenerate(object app, object modelDoc, object feature)
        {
            Init(app, modelDoc, feature);
            return Regenerate();
        }

        public object Security(object app, object modelDoc, object feature)
        {
            Init(app, modelDoc, feature);
            return Security();
        }

        private static void SaveSelections(MacroFeatureBase<TMacroFeature,TData> sampleMacroFeature)
        {
            var objects = sampleMacroFeature.SelectionMgr.GetSelectedObjects((type, mark) => true)
                .Cast<IBody2>()
                .ToArray();

            var marks =
                Enumerable.Range(1, objects.Length)
                    .Select(i => sampleMacroFeature.SelectionMgr.GetSelectedObjectMark(i))
                    .ToArray();

            sampleMacroFeature.SwFeatureData.SetSelections(ComWangling.ObjectArrayToDispatchWrapper(objects), marks);
            Debug.Assert(sampleMacroFeature.SwFeatureData.GetSelectionCount() == objects.Length);
        }

        protected void LoadSelections()
        {
            if (SwFeatureData != null)
            {
                var result = SwFeatureData.AccessSelections(ModelDoc, null);
                if (!result)
                    throw new Exception("Expected to get true");
                {
                    object objects;
                    object objectTypes;
                    object marks;
                    object drViews;
                    object componentXForms;
                    SwFeatureData.GetSelections3(out objects, out objectTypes, out marks, out drViews, out componentXForms);

                    if (objects != null)
                    {
                        var objectsArray = ((object[]) objects).Cast<IBody2>().ToList();
                        var typesArray = (swSelectType_e[]) objectTypes;

                        ModelDoc.ClearSelection2(true);
                        foreach (var feature in objectsArray)
                        {
                            feature.Select2(true, null);
                        }
                    }
                }
            }
        }


        public void Commit()
        {
            if (State==StateEnum.Insert)
            {
                InsertDefinition(FeatureName, EditBodies, (int) FeatureOptions);
            }
            else
            {
                ModifyDefinition();
            }
        }
    }
}