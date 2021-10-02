using System;
using System.IO;
using Cake.Core.IO;

namespace Cake.LocalTools.Module
{
    internal interface IToolManifestReader
    {
        /// <summary>
        /// Loads a .NET Local Tool manifest from the specified file
        /// </summary>
        /// <param name="file">The tool manifest file (<c>dotnet-tools.json</c>)</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="file"/> is <c>null</c></exception>
        /// <exception cref="FileNotFoundException">Thrown when the specified file does not exist.</exception>
        /// <exception cref="InvalidToolManifestException">Thrown when the specified file is not a valid or supported .NET Local Tool manifest.</exception>
        ToolManifest Load(IFile file);
    }
}
