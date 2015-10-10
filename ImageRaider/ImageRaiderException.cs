using System;
using System.Runtime.Serialization;

namespace ImageRaider
{
    [Serializable]
    internal class ImageRaiderException : Exception
    {
        private object p;

        public ImageRaiderException()
        {
        }

        public ImageRaiderException(string message) : base(message)
        {
        }

        public ImageRaiderException(object p)
        {
            this.p = p;
        }

        public ImageRaiderException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ImageRaiderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}