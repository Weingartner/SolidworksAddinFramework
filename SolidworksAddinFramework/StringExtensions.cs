using System.Text.RegularExpressions;

namespace SolidworksAddinFramework
{
    public static class StringExtensions
    {
        public static string Abbreviate(this string s) => Regex.Replace(s, @"[^\p{Lu}]", "");
        public static string CamelCaseToHumanReadable(this string s) => Regex.Replace(s, @"(?<!^)(\p{Lu})", m => " " + m.Value.ToLower());
    }
}
