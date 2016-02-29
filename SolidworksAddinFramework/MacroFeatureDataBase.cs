using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidworksAddinFramework
{
    /// <summary>
    /// Base class for any macro feature parameter set. Mark an string, double, int or bool property with attribute 
    /// 'MacroFeatureDataField' and it will be automagically serialized to the macro feature macroFeatureData.
    /// </summary>
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
                if (p.PropertyType == typeof (int))
                {
                    return swMacroFeatureParamType_e.swMacroFeatureParamTypeInteger;

                }
                if (p.PropertyType == typeof (double))
                {
                    return swMacroFeatureParamType_e.swMacroFeatureParamTypeDouble;
                }
                if (p.PropertyType == typeof (bool))
                {
                    return swMacroFeatureParamType_e.swMacroFeatureParamTypeInteger;
                }
                throw new InvalidCastException($"Cannot bind the type {p.PropertyType.Name} ");
            })
                .Select(v=>(int)v)
                .ToArray();
            _Names = BindingProperties.Select(p => p.Name).ToArray();
            _Values = BindingProperties.Select(p => p.GetValue(this, new object[] {})).ToArray();
        }

        /// <summary>
        /// Copy the database to the _Values array
        /// </summary>
        private void UpdateValues()
        {
            var tmp = BindingProperties.Select(p => p.GetValue(this, new object[] {})).ToList();
            tmp.CopyTo(_Values);

        }

        /// <summary>
        /// List of database properties 
        /// </summary>
        private List<PropertyInfo> BindingProperties
        {
            get
            {
                return GetType().GetProperties()
                    .Where(p => p.GetCustomAttributes(typeof (MacroFeatureDataFieldAttribute), true).Length > 0)
                    .ToList();
            }
        }

        /// <summary>
        /// Macro feature macroFeatureData parameter names
        /// </summary>
        public string[] Names => _Names;

        /// <summary>
        /// Macro feature macroFeatureData parameter types
        /// </summary>
        public int[] Types => _Types;

        /// <summary>
        /// Macro feature macroFeatureData parameter values.
        /// </summary>
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

        /// <summary>
        /// Write the database to the macro feature macroFeatureData.
        /// </summary>
        /// <param name="macroFeatureData"></param>
        public void WriteTo(IMacroFeatureData macroFeatureData)
        {
            foreach (var bindingProperty in BindingProperties)
            {
                if (bindingProperty.PropertyType == typeof (string))
                {
                    macroFeatureData.SetStringByName(bindingProperty.Name, (string) bindingProperty.GetValue(this,new object[] {}));
                }else if (bindingProperty.PropertyType == typeof (int))
                {
                    macroFeatureData.SetIntegerByName(bindingProperty.Name, (int) bindingProperty.GetValue(this,new object[] {}));
                    
                }else if (bindingProperty.PropertyType == typeof (double))
                {
                    macroFeatureData.SetDoubleByName(bindingProperty.Name, (double) bindingProperty.GetValue(this,new object[] {}));
                }
                else if (bindingProperty.PropertyType == typeof (bool))
                {
                    var value = (bool) bindingProperty.GetValue(this, new object[] {});
                    macroFeatureData.SetIntegerByName(bindingProperty.Name, value ? 1 : 0);
                }
                else
                {
                    throw new InvalidCastException($"Cannot bind the type {bindingProperty.PropertyType.Name} ");
                }
            }
        }

        /// <summary>
        /// Read the values from the macro feature data and store in the database
        /// </summary>
        /// <param name="data"></param>
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
}