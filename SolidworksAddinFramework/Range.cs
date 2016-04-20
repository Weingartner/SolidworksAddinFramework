using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SolidworksAddinFramework
{
    public struct Range :IEnumerable<Double>
    {
        private readonly double _Min;
        private readonly double _Max;


        public Range(IEnumerable<double> values)
        {
            _Min = Double.MaxValue;
            _Max = Double.MinValue;
            foreach (var value in values)
            {
                _Min = Math.Min(value, _Min);
                _Max = Math.Max(value, _Max);
            }
        }

        public double Min
        {
            get { return _Min; }
        }

        public double Max
        {
            get { return _Max; }
        }

        public static Range MaxRange()
        {
            return new Range(double.MinValue, double.MaxValue);
        }


        public Range(double min, double max)
        {
            Debug.Assert(min <= max);
            _Min = min;
            _Max = max;

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Range operator +(Range range, double value)
        {
            // Not implemented with Min Max for performance reasons
            var oldMin = range._Min;
            var oldMax = range._Max;
            var min = value < oldMin ? value : oldMin;
            var max = value > oldMax ? value : oldMax;
            return new Range(min, max);
        }

        public bool Overlaps(Range other)
        {
            if (_Max < other._Min)
                return false;
            if (_Min > other._Max)
                return false;
            return true;
        }

        public Range Intersect(Range other)
        {
            var min = Math.Max(_Min, other._Min);
            var max = Math.Min(_Max, other._Max);

            return new Range(min, max);
        }

        public bool Contains(double value)
        {
            return value > _Min && value < _Max;
        }

        public IEnumerator<double> GetEnumerator()
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