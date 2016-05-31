using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;

namespace SolidworksAddinFramework
{
    public static class BoolExtensions
    {
        public static Option<T> IfTrue<T>(this bool v, Func<T> fn) => v ? Some(fn()) : None;
        public static Option<T> IfTrue<T>(this bool v, T t) => v.IfTrue(() => t);
    }
}
