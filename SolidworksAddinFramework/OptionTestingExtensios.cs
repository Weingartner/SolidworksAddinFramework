using System;
using LanguageExt;

namespace SolidworksAddinFramework
{
    public static class OptionTestingExtensios
    {
        /// <summary>
        /// Get the value from an option. If it is none 
        /// then a null reference exception will be raised.
        /// Don't use this in production please.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="option"></param>
        /// <returns></returns>
        public static T __Value__<T>(this Option<T> option)
        {
            return option.Match
                (v => v ,
                    () =>
                    {
                        throw new NullReferenceException();
                    });
        }
    }
}