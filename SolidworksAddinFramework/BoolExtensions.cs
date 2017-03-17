using System;
using LanguageExt;
using static LanguageExt.Prelude;

namespace SolidworksAddinFramework
{
    public static class BoolExtensions
    {
        public static Option<T> IfTrue<T>(this bool v, Func<T> fn) => v ? Some(fn()) : None;
        public static bool IfTrue(this bool v, Action fn)
        {
            if (v)
                fn();
            return true;
        }
        public static Option<T> IfTrue<T>(this bool v, T t) => v.IfTrue(() => t);
    }
}
