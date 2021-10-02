using System.Collections.Generic;
using Cake.Core.IO;
using Cake.Core.Packaging;
using Cake.DotNetTool.Module;

namespace Cake.LocalTools.Module
{
    /// <summary>
    /// Interface for <see cref="DotNetToolPackageInstaller" />.
    /// </summary>
    /// <remarks>
    /// This interface allows calling <see cref="DotNetToolPackageInstaller"/> from <see cref="ToolManifestPackageInstaller"/>
    /// without having to take a direct dependency to the implementation class.
    /// This is enables testing of <see cref="ToolManifestPackageInstaller" /> without calling into the real implementation.
    /// <para>
    /// The default implementation is <see cref="DotNetToolPackageInstallerWrapper"/> which just create a instance of <see cref="DotNetToolPackageInstaller"/>.
    /// </para>
    /// </remarks>
    internal interface IDotNetToolPackageInstaller
    {
        IReadOnlyCollection<IFile> Install(PackageReference package, PackageType type, DirectoryPath path);
    }
}
