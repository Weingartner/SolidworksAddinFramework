using LanguageExt.Parsec;
using Char = LanguageExt.Parsec.Char;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Char;

namespace SolidworksAddinFramework
{
    /// <summary>
    /// Usefull extensions method for the LanguageExt.Parsec library
    /// </summary>
    public static class ParserExtensions
    {
        public static Parser<TLeft> keepLeft<TLeft, TRight>(this Parser<TLeft> l, Parser<TRight> r) => from x in l from y in r select x;
        public static Parser<T> padded<T>(Parser<T> p) => Prim.between(Char.spaces, Char.spaces, p);
        public static Parser<TLeft> skip<TLeft, TRight>(this Parser<TLeft> l, Parser<TRight> r) => l.keepLeft(r);
        public static Parser<TLeft> skipWhite<TLeft>(this Parser<TLeft> l) => l.keepLeft(spaces);
        public static Parser<TRight> keepRight<TLeft, TRight>(this Parser<TLeft> l, Parser<TRight> r) => from x in l from y in r select y;
        public static Parser<T> surrounded<T, U>(this Parser<U> u, Parser<T> t) => between(u, u, t);
        public static Parser<T> doubleQuoted<T>(this Parser<T> t) => surrounded(ch('"'), t);

    }
}
