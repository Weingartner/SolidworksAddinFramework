using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using LanguageExt.UnitsOfMeasure;
using Newtonsoft.Json;
using SolidworksAddinFramework.Wpf;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using Weingartner.Numerics;
using Weingartner.ReactiveCompositeCollections;

namespace SolidworksAddinFramework
{
    [DataContract]
    public class SwEq
    {
        protected bool Equals(SwEq other)
        {
            return string.Equals(Id, other.Id) && Val.Equals(other.Val) && UnitsType == other.UnitsType && string.Equals(SolidWorksUnits, other.SolidWorksUnits);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((SwEq) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ Val.GetHashCode();
                hashCode = (hashCode*397) ^ (int) UnitsType;
                hashCode = (hashCode*397) ^ (SolidWorksUnits != null ? SolidWorksUnits.GetHashCode() : 0);
                return hashCode;
            }
        }

        [DataMember]
        public string Id { get; }
        [DataMember]
        public double Val { get; }
        [DataMember]
        public UnitsEnum UnitsType { get; }
        /// <summary>
        /// The units that the value will be written to solidworks as. The Val
        /// property is always stored as meters or radians.
        /// </summary>
        [DataMember]
        public string SolidWorksUnits { get; }

        private static IDisposable ConfigureLinearAxisNumberBox(IPropertyManagerPageNumberbox config, double increment)
        {
            config.SetRange2((int)swNumberboxUnitType_e.swNumberBox_Length, -10, 10, true, increment, increment * 10, increment / 10);
            config.DisplayedUnit = (int)swLengthUnit_e.swMM;
            return Disposable.Empty;
        }

        public static IDisposable CreateControl(PropertyManagerPageBase pmp, IPropertyManagerPageGroup @group, CompositeSourceList<SwEq> list, int index)
        {
            var equation = list.Source[index];

            var caption = equation.Id.CamelCaseToHumanReadable();
            var label = pmp.CreateLabel(@group, caption, caption);
            var id = pmp.NextId();
            var box = @group.CreateNumberBox(id, caption, caption);

            if (equation.UnitsType == UnitsEnum.Angle)
            {
                box.SetRange2((int) swNumberboxUnitType_e.swNumberBox_Angle, -10, 10, true, 0.005, 0.010, 0.001);
                box.DisplayedUnit = (int) swAngleUnit_e.swDEGREES;
            }
            else
            {
                const double increment = 1e-2;
                box.SetRange2((int)swNumberboxUnitType_e.swNumberBox_Length, -10, 10, true, increment, increment * 10, increment / 10);
                box.DisplayedUnit = (int)swLengthUnit_e.swMM;
            }
            var obs = pmp.NumberBoxChangedObservable(id);
            var d2 = obs.Subscribe(value => list.ReplaceAt(index,equation.WithValue(value)));

            var d3 = list.ChangesObservable()
                .Select(_=>Unit.Default)
                .StartWith(Unit.Default)
                .Subscribe(v => box.Value = list.Source[index].Val);

            return ControlHolder.Create(@group, box, d2, label, d3);
        }

        public static IDisposable CreateControls(PropertyManagerPageBase pmp,
            IPropertyManagerPageGroup @group,
            CompositeSourceList<SwEq> list)
        {
            var d = new CompositeDisposable();
            for (var i = 0; i < list.Source.Count; i++)
            {
                d.Add(CreateControl(pmp, @group, list, i));
            }
            return d;
        }

        private SwEq WithValue(double value) => new SwEq(Id, value, SolidWorksUnits, UnitsType);

        [JsonConstructor]
        private SwEq(string id, double val, string solidWorksUnits, UnitsEnum unitsType)
        {
            Id = id;
            Val = val;
            SolidWorksUnits = solidWorksUnits;
            UnitsType = unitsType;
        }

        public SwEq(string id, double val, string solidWorksUnits )
        {
            Id = id;
            Val = val;
            SolidWorksUnits = solidWorksUnits;
            switch (SolidWorksUnits)
            {
                case "cm": // centimeters
                    Val = Val.Centimetres().Metres;
                    UnitsType = UnitsEnum.Length;
                    break;
                case "ft": // feet
                    Val = Val.Feet().Metres;
                    UnitsType = UnitsEnum.Length;
                    break;
                case "in": // inches
                    Val = Val.Inches().Metres;
                    UnitsType = UnitsEnum.Length;
                    break;
                case "m":  // meters
                    Val = Val.Metres().Metres;
                    UnitsType = UnitsEnum.Length;
                    break;
                case "uin":// micro inches
                    Val = (Val/1e9).Inches().Metres;
                    UnitsType = UnitsEnum.Length;
                    break;
                case "um": // micro meteres
                    Val = Val.Micrometres().Metres;
                    UnitsType = UnitsEnum.Length;
                    break;
                case "mil": // thousanth of an inch
                    Val = (Val/1e6).Inches().Metres;
                    UnitsType = UnitsEnum.Length;
                    break;
                case "mm": // millimeteres
                    Val = Val.Millimetres().Metres;
                    UnitsType = UnitsEnum.Length;
                    break;
                case "nm": // nanometers
                    Val = Val.Nanometres().Metres;
                    UnitsType = UnitsEnum.Length;
                    break;
                case "deg": // degrees
                    Val = Val*Math.PI/180;
                    UnitsType = UnitsEnum.Angle;
                    break;
                case "rad": // radians
                    Val = Val;
                    UnitsType = UnitsEnum.Angle;
                    break;
                case "undefined":
                default:
                    throw new Exception($"Not supported {SolidWorksUnits}");
                    
            }
        }


        public override string ToString() => $@"""{Id}""={GetValUnits()}";
        public string ToLongString() => $@"{Id.CamelCaseToHumanReadable()} = {GetValUnits()}";
        public string ToShortString() => $@"{Id.Abbreviate()} = {GetValUnits(3)}";

        public string GetValUnits(int sigFigs = 0)
        {
            var scaled = 0.0;
            switch (SolidWorksUnits)
            {
                case "cm": // centimeters
                    scaled = Val.Metres().Centimetres;
                    break;
                case "ft": // feet
                    scaled = Val.Metres().Feet;
                    break;
                case "in": // inches
                    scaled = Val.Metres().Inches;
                    break;
                case "m": // meters
                    scaled = Val.Metres().Metres;
                    break;
                case "uin": // micro inches
                    scaled = Val.Metres().Inches*1e9;
                    break;
                case "um": // micro meteres
                    scaled = Val.Metres().Micrometres;
                    break;
                case "mil": // thousanth of an inch
                    scaled = Val.Metres().Inches*1e6;
                    break;
                case "mm": // millimeteres
                    scaled = Val.Metres().Millimetres;
                    break;
                case "nm": // nanometers
                    scaled = Val.Metres().Nanometres;
                    break;
                case "deg": // degrees
                    scaled = Val*180/Math.PI;
                    break;
                case "rad": // radians
                    scaled = Val;
                    break;
                default:
                    throw new Exception($"Not supported {SolidWorksUnits}");
            }

            var str = sigFigs == 0 
                ? scaled.ToString() : ((decimal) scaled).RoundToSignificantDigits((short)sigFigs).ToString(CultureInfo.InvariantCulture);
            return $"{str}{SolidWorksUnits}";
        }
    }
}