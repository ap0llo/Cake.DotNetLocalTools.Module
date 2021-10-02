using System;

namespace Cake.DotNetLocalTools.Module
{
    public sealed class InvalidToolManifestException : Exception
    {
        public InvalidToolManifestException(string message) : base(message)
        { }

        public InvalidToolManifestException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}
