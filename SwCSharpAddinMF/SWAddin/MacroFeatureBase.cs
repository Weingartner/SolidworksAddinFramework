using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;
using Attribute = SolidWorks.Interop.sldworks.Attribute;

namespace SwCSharpAddinMF.SWAddin
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MacroFeatureDataFieldAttribute : System.Attribute
    {
        
    }

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
                return this.GetType().GetProperties()
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
                    bool value = (bool) bindingProperty.GetValue(this, new object[] {});
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

    public abstract class MacroFeatureBase<T> : ISwComFeature
        where T : MacroFeatureDataBase, new()
    {
        public IModelDoc2 ModelDoc { get; set; }

        public IFeature SwFeature { get; set; }

        public IMacroFeatureData SwFeatureData { get; set; }

        public ISldWorks SwApp { get; set; }
        public abstract T Database { get; set; }

        public object Edit(object app, object modelDoc, object feature)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            if (modelDoc == null) throw new ArgumentNullException(nameof(modelDoc));
            if (feature == null) throw new ArgumentNullException(nameof(feature));

            SwApp = app as ISldWorks;
            SwFeature = feature as IFeature;
            SwFeatureData = (IMacroFeatureData) SwFeature.GetDefinition();
            ModelDoc = modelDoc as IModelDoc2;
            Database = new T();
            Database.ReadFrom(SwFeatureData);
            return Edit();
        }

        public void Write()
        {
            Database.WriteTo(SwFeatureData);
        }

        public void ModifyDefinition()
        {
            Write();
            SwFeature.ModifyDefinition(SwFeatureData, ModelDoc, null);
        }

        public void ReleaseSelectionAccess()
        {
            SwFeatureData.ReleaseSelectionAccess();
        }


        protected abstract object Edit();
        protected abstract object Security();
        protected abstract object Regenerate();

        public object Regenerate(object app, object modelDoc, object feature)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            if (modelDoc == null) throw new ArgumentNullException(nameof(modelDoc));
            if (feature == null) throw new ArgumentNullException(nameof(feature));
            
            SwApp = app as ISldWorks;
            SwFeature = feature as IFeature;
            ModelDoc = modelDoc as IModelDoc2;
            return Regenerate();
        }

        public object Security(object app, object modelDoc, object feature)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            if (modelDoc == null) throw new ArgumentNullException(nameof(modelDoc));
            if (feature == null) throw new ArgumentNullException(nameof(feature));

            SwApp = app as ISldWorks;
            SwFeature = feature as IFeature;
            ModelDoc = modelDoc as IModelDoc2;
            return Security();
        }
    }
}