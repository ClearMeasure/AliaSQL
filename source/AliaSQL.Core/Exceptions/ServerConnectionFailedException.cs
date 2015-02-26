using System;

namespace AliaSQL.Core.Exceptions
{
    [Serializable]
    public class ServerConnectionFailedException : Exception
    {
        public ServerConnectionFailedException() { }

        public ServerConnectionFailedException(string message) : base(message) { }

        public ServerConnectionFailedException(string message, Exception inner) : base(message, inner) { }

        protected ServerConnectionFailedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
