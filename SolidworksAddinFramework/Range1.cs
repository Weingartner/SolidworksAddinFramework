

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SolidworksAddinFramework {

    
    public struct RangeSingle :IEnumerable<Single>
    {
        private readonly Single _Min;
        private readonly Single _Max;


        public RangeSingle(IEnumerable<Single> values)
        {
            _Min = Single.MaxValue;
            _Max = Single.MinValue;
            foreach (var value in values)
            {
                _Min = Math.Min(value, _Min);
                _Max = Math.Max(value, _Max);
            }
        }

        public Single Min
        {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _Min; }
        }

        public Single Max
        {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _Max; }
        }

        public static RangeSingle MaxRange()
        {
            return new RangeSingle(Single.MinValue, Single.MaxValue);
        }


        public RangeSingle(Single min, Single max)
        {
            Debug.Assert(min <= max);
            _Min = min;
            _Max = max;

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RangeSingle operator +(RangeSingle range, Single value)
        {
            // Not implemented with Min Max for performance reasons
            var oldMin = range._Min;
            var oldMax = range._Max;
            var min = value < oldMin ? value : oldMin;
            var max = value > oldMax ? value : oldMax;
            return new RangeSingle(min, max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Overlaps(RangeSingle other)
        {
            if (_Max < other._Min)
                return false;
            if (_Min > other._Max)
                return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RangeSingle Intersect(RangeSingle other)
        {
            var min = Math.Max(_Min, other._Min);
            var max = Math.Min(_Max, other._Max);

            return new RangeSingle(min, max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Single value)
        {
            return value > _Min && value < _Max;
        }

        public IEnumerator<Single> GetEnumerator()
        {
            yield return _Min;
            yield return _Max;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }



    
    public struct RangeDouble :IEnumerable<Double>
    {
        private readonly Double _Min;
        private readonly Double _Max;


        public RangeDouble(IEnumerable<Double> values)
        {
            _Min = Double.MaxValue;
            _Max = Double.MinValue;
            foreach (var value in values)
            {
                _Min = Math.Min(value, _Min);
                _Max = Math.Max(value, _Max);
            }
        }

        public Double Min
        {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _Min; }
        }

        public Double Max
        {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _Max; }
        }

        public static RangeDouble MaxRange()
        {
            return new RangeDouble(Double.MinValue, Double.MaxValue);
        }


        public RangeDouble(Double min, Double max)
        {
            Debug.Assert(min <= max);
            _Min = min;
            _Max = max;

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RangeDouble operator +(RangeDouble range, Double value)
        {
            // Not implemented with Min Max for performance reasons
            var oldMin = range._Min;
            var oldMax = range._Max;
            var min = value < oldMin ? value : oldMin;
            var max = value > oldMax ? value : oldMax;
            return new RangeDouble(min, max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Overlaps(RangeDouble other)
        {
            if (_Max < other._Min)
                return false;
            if (_Min > other._Max)
                return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RangeDouble Intersect(RangeDouble other)
        {
            var min = Math.Max(_Min, other._Min);
            var max = Math.Min(_Max, other._Max);

            return new RangeDouble(min, max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Double value)
        {
            return value > _Min && value < _Max;
        }

        public IEnumerator<Double> GetEnumerator()
        {
            yield return _Min;
            yield return _Max;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }



    
    public struct RangeInt64 :IEnumerable<Int64>
    {
        private readonly Int64 _Min;
        private readonly Int64 _Max;


        public RangeInt64(IEnumerable<Int64> values)
        {
            _Min = Int64.MaxValue;
            _Max = Int64.MinValue;
            foreach (var value in values)
            {
                _Min = Math.Min(value, _Min);
                _Max = Math.Max(value, _Max);
            }
        }

        public Int64 Min
        {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _Min; }
        }

        public Int64 Max
        {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _Max; }
        }

        public static RangeInt64 MaxRange()
        {
            return new RangeInt64(Int64.MinValue, Int64.MaxValue);
        }


        public RangeInt64(Int64 min, Int64 max)
        {
            Debug.Assert(min <= max);
            _Min = min;
            _Max = max;

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RangeInt64 operator +(RangeInt64 range, Int64 value)
        {
            // Not implemented with Min Max for performance reasons
            var oldMin = range._Min;
            var oldMax = range._Max;
            var min = value < oldMin ? value : oldMin;
            var max = value > oldMax ? value : oldMax;
            return new RangeInt64(min, max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Overlaps(RangeInt64 other)
        {
            if (_Max < other._Min)
                return false;
            if (_Min > other._Max)
                return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RangeInt64 Intersect(RangeInt64 other)
        {
            var min = Math.Max(_Min, other._Min);
            var max = Math.Min(_Max, other._Max);

            return new RangeInt64(min, max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Int64 value)
        {
            return value > _Min && value < _Max;
        }

        public IEnumerator<Int64> GetEnumerator()
        {
            yield return _Min;
            yield return _Max;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }



    
    public struct RangeUInt16 :IEnumerable<UInt16>
    {
        private readonly UInt16 _Min;
        private readonly UInt16 _Max;


        public RangeUInt16(IEnumerable<UInt16> values)
        {
            _Min = UInt16.MaxValue;
            _Max = UInt16.MinValue;
            foreach (var value in values)
            {
                _Min = Math.Min(value, _Min);
                _Max = Math.Max(value, _Max);
            }
        }

        public UInt16 Min
        {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _Min; }
        }

        public UInt16 Max
        {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _Max; }
        }

        public static RangeUInt16 MaxRange()
        {
            return new RangeUInt16(UInt16.MinValue, UInt16.MaxValue);
        }


        public RangeUInt16(UInt16 min, UInt16 max)
        {
            Debug.Assert(min <= max);
            _Min = min;
            _Max = max;

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RangeUInt16 operator +(RangeUInt16 range, UInt16 value)
        {
            // Not implemented with Min Max for performance reasons
            var oldMin = range._Min;
            var oldMax = range._Max;
            var min = value < oldMin ? value : oldMin;
            var max = value > oldMax ? value : oldMax;
            return new RangeUInt16(min, max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Overlaps(RangeUInt16 other)
        {
            if (_Max < other._Min)
                return false;
            if (_Min > other._Max)
                return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RangeUInt16 Intersect(RangeUInt16 other)
        {
            var min = Math.Max(_Min, other._Min);
            var max = Math.Min(_Max, other._Max);

            return new RangeUInt16(min, max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(UInt16 value)
        {
            return value > _Min && value < _Max;
        }

        public IEnumerator<UInt16> GetEnumerator()
        {
            yield return _Min;
            yield return _Max;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }



    
    public struct RangeUInt32 :IEnumerable<UInt32>
    {
        private readonly UInt32 _Min;
        private readonly UInt32 _Max;


        public RangeUInt32(IEnumerable<UInt32> values)
        {
            _Min = UInt32.MaxValue;
            _Max = UInt32.MinValue;
            foreach (var value in values)
            {
                _Min = Math.Min(value, _Min);
                _Max = Math.Max(value, _Max);
            }
        }

        public UInt32 Min
        {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _Min; }
        }

        public UInt32 Max
        {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _Max; }
        }

        public static RangeUInt32 MaxRange()
        {
            return new RangeUInt32(UInt32.MinValue, UInt32.MaxValue);
        }


        public RangeUInt32(UInt32 min, UInt32 max)
        {
            Debug.Assert(min <= max);
            _Min = min;
            _Max = max;

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RangeUInt32 operator +(RangeUInt32 range, UInt32 value)
        {
            // Not implemented with Min Max for performance reasons
            var oldMin = range._Min;
            var oldMax = range._Max;
            var min = value < oldMin ? value : oldMin;
            var max = value > oldMax ? value : oldMax;
            return new RangeUInt32(min, max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Overlaps(RangeUInt32 other)
        {
            if (_Max < other._Min)
                return false;
            if (_Min > other._Max)
                return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RangeUInt32 Intersect(RangeUInt32 other)
        {
            var min = Math.Max(_Min, other._Min);
            var max = Math.Min(_Max, other._Max);

            return new RangeUInt32(min, max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(UInt32 value)
        {
            return value > _Min && value < _Max;
        }

        public IEnumerator<UInt32> GetEnumerator()
        {
            yield return _Min;
            yield return _Max;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }



    
    public struct RangeUInt64 :IEnumerable<UInt64>
    {
        private readonly UInt64 _Min;
        private readonly UInt64 _Max;


        public RangeUInt64(IEnumerable<UInt64> values)
        {
            _Min = UInt64.MaxValue;
            _Max = UInt64.MinValue;
            foreach (var value in values)
            {
                _Min = Math.Min(value, _Min);
                _Max = Math.Max(value, _Max);
            }
        }

        public UInt64 Min
        {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _Min; }
        }

        public UInt64 Max
        {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _Max; }
        }

        public static RangeUInt64 MaxRange()
        {
            return new RangeUInt64(UInt64.MinValue, UInt64.MaxValue);
        }


        public RangeUInt64(UInt64 min, UInt64 max)
        {
            Debug.Assert(min <= max);
            _Min = min;
            _Max = max;

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RangeUInt64 operator +(RangeUInt64 range, UInt64 value)
        {
            // Not implemented with Min Max for performance reasons
            var oldMin = range._Min;
            var oldMax = range._Max;
            var min = value < oldMin ? value : oldMin;
            var max = value > oldMax ? value : oldMax;
            return new RangeUInt64(min, max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Overlaps(RangeUInt64 other)
        {
            if (_Max < other._Min)
                return false;
            if (_Min > other._Max)
                return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RangeUInt64 Intersect(RangeUInt64 other)
        {
            var min = Math.Max(_Min, other._Min);
            var max = Math.Min(_Max, other._Max);

            return new RangeUInt64(min, max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(UInt64 value)
        {
            return value > _Min && value < _Max;
        }

        public IEnumerator<UInt64> GetEnumerator()
        {
            yield return _Min;
            yield return _Max;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }



    }