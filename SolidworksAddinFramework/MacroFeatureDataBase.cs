using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Reactive.Bindings;
using ReactiveUI;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidworksAddinFramework
{
    /// <summary>
    /// Base class for any macro feature parameter set. Mark an string, double, int or bool property with attribute 
    /// 'MacroFeatureDataField' and it will be automagically serialized to the macro feature macroFeatureData.
    /// </summary>
    public class MacroFeatureDataBase : ReactiveObject
    {
        private int[] _Types;
        private string[] _Names;
        private object[] _Values;

        public MacroFeatureDataBase()
        {
            _Types = GetTypes().Select(v=>(int)v).ToArray();
            _Names = BindingProperties.Select(p => p.Name).ToArray();
            _Values = BindingProperties.Select(p => p.GetValue(this, new object[] {})).ToArray();
        }

        /// <summary>
        /// Copy the database to the _Values array
        /// </summary>
        private void UpdateValues()
        {
            var tmp = BindingProperties.Select(p =>
            {
                if (IsReactiveProperty(p.PropertyType))
                {
                    var reactiveProperty = p.GetValue(this, new object[0]);
                    return
                        reactiveProperty.GetType()
                            .GetProperty(nameof(ReactiveProperty<object>.Value))
                            .GetValue(reactiveProperty, new object[0]);
                }
                return p.GetValue(this, new object[] {});
            }).ToList();
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

        public void WriteTo(IMacroFeatureData macroFeatureData)
        {
            IEnumerable<PropertyInfo> bindingProperties = BindingProperties;
            foreach (var bindingProperty in bindingProperties)
            {
                var o = bindingProperty.GetValue(this,new object[] {});
                var propertyType = bindingProperty.PropertyType;
                if (IsReactiveProperty(propertyType))
                {
                    var valuePropertyName = nameof(ReactiveProperty<object>.Value);
                    var valuePropertyInfo = o.GetType().GetProperty(valuePropertyName);
                    var value = valuePropertyInfo.GetValue(o, new object[] {});
                    WriteTo(macroFeatureData, valuePropertyInfo.PropertyType, bindingProperty.Name, value);
                }
                else
                {
                    WriteTo(macroFeatureData, propertyType, bindingProperty.Name, o);
                }
            }
        }

        private static bool IsReactiveProperty(Type propertyType)
        {
            return propertyType.IsGenericType &&
                   propertyType.GetGenericTypeDefinition() == typeof (ReactiveProperty<>);
        }

        private IEnumerable<swMacroFeatureParamType_e> GetTypes()
        {
            IEnumerable<PropertyInfo> bindingProperties = BindingProperties;
            foreach (var bindingProperty in bindingProperties)
            {
                var o = bindingProperty.GetValue(this,new object[] {});
                var propertyType = bindingProperty.PropertyType;
                if (IsReactiveProperty(propertyType))
                {
                    var valuePropertyName = nameof(ReactiveProperty<object>.Value);
                    var valuePropertyInfo = o.GetType().GetProperty(valuePropertyName);
                    var value = valuePropertyInfo.GetValue(o, new object[] {});
                    yield return Type(valuePropertyInfo);
                }
                else
                {
                    yield return Type(bindingProperty);
                }
            }
        }

        swMacroFeatureParamType_e Type(PropertyInfo p)
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
        }

        private static void WriteTo(IMacroFeatureData macroFeatureData, Type propertyType, string paramName, object o)
        {
            if (propertyType == typeof (string))
            {
                macroFeatureData.SetStringByName(paramName, (string) o);
            }
            else if (propertyType == typeof (int))
            {
                macroFeatureData.SetIntegerByName(paramName, (int) o);
            }
            else if (propertyType == typeof (double))
            {
                macroFeatureData.SetDoubleByName(paramName, (double) o);
            }
            else if (propertyType == typeof (bool))
            {
                var value = (bool) o;
                macroFeatureData.SetIntegerByName(paramName, value ? 1 : 0);
            }
            else
            {
                throw new InvalidCastException($"Cannot bind the type {propertyType.Name} ");
            }
        }

        public void ReadFrom(IMacroFeatureData macroFeatureData)
        {
            foreach (var bindingProperty in (IEnumerable<PropertyInfo>) BindingProperties)
            {
                var propertyType = bindingProperty.PropertyType;

                var paramName = bindingProperty.Name;

                if (IsReactiveProperty(propertyType))
                {
                    var reactiveProperty = bindingProperty.GetValue(this, new object[] {});

                    var valuePropertyName = nameof(ReactiveProperty<object>.Value);
                    var valueProperty = reactiveProperty.GetType().GetProperty(valuePropertyName);
                    ReadValue(reactiveProperty, valueProperty, macroFeatureData, paramName);
                }else
                {
                    ReadValue(this, bindingProperty, macroFeatureData, paramName);
                    
                }
            }
        }

        private static void ReadValue(object o, PropertyInfo property, IMacroFeatureData macroFeatureData, string paramName)
        {
            var propertyType = property.PropertyType;

            if (propertyType == typeof (string))
            {
                string v;
                macroFeatureData.GetStringByName(paramName, out v);
                property.SetValue(o, v, new object[] {});
            }
            else if (propertyType == typeof (int))
            {
                int v;
                macroFeatureData.GetIntegerByName(paramName, out v);
                property.SetValue(o, v, new object[] {});
            }
            else if (propertyType == typeof (double))
            {
                double v;
                macroFeatureData.GetDoubleByName(paramName, out v);
                property.SetValue(o, v, new object[] {});
            }
            else if (propertyType == typeof (bool))
            {
                int v;
                macroFeatureData.GetIntegerByName(paramName, out v);
                property.SetValue(o, v == 1, new object[] {});
            }
            else
            {
                throw new InvalidCastException($"Cannot bind the type {propertyType.Name} ");
            }
        }
    }
}