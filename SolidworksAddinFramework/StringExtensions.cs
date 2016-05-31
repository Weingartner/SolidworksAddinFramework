using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SolidworksAddinFramework
{
    public static class StringExtensions
    {
        public static string Abbreviate(this string s) => new Regex(@"[^\p{Lu}]").Replace(s, "");
    }
}
