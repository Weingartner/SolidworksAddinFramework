﻿using System.DoubleNumerics;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace SolidworksAddinFramework.Spec.FluentAssertions
{
    public static class Extensions
    {

        
        public static AndConstraint<ObjectAssertions> BeApproximately
            (
            this ObjectAssertions parent,
            Vector3 expectedValue,
            double precision,
            string because = "",
            params object[] becauseArgs)
        {

            var ps = (Vector3) parent.Subject;
            var dx = (expectedValue - ps).Length();

            Execute.Assertion.ForCondition(dx <= precision)
                .BecauseOf(because, becauseArgs)
                .FailWith(
                    "Expected value {0} to approximate {1} +/- {2}{reason}, but it differed by {3}.",
                    (object) parent.Subject,
                    (object) expectedValue,
                    (object) precision,
                    (object) dx);

            return new AndConstraint<ObjectAssertions>(parent);
        }
    }
}
