using System;

namespace Mifs.MEX.Authentication
{
    internal class MEXAuthenticationException : Exception
    {
        public MEXAuthenticationException(string message) : base(message)
        {
        }

        public MEXAuthenticationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
