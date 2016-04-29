using System;
using System.Runtime.Serialization;

namespace SolidworksAddinFramework
{
    [Serializable]
    public class SelectionException : Exception
    {
        public SelectionException()
        {
        }

        public SelectionException(string message) : base(message)
        {
        }

        public SelectionException(string message, Exception inner) : base(message, inner)
        {
        }

        protected SelectionException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}