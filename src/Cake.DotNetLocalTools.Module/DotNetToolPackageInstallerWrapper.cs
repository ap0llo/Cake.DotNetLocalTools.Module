using System.Collections.Generic;
using Cake.Core;
using Cake.Core.Configuration;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Core.Packaging;
using Cake.DotNetTool.Module;

namespace Cake.DotNetLocalTools.Module
{
    /// <summary>
    /// Adapter class to add the <see cref="IDotNetToolPackageInstaller"/> interface to <see cref="DotNetToolPackageInstaller"/>.
    /// </summary>
    internal class DotNetToolPackageInstallerWrapper : IDotNetToolPackageInstaller
    {
        private readonly DotNetToolPackageInstaller m_Implementation;

        public DotNetToolPackageInstallerWrapper(
            ICakeEnvironment environment,
            IProcessRunner processRunner,
            ICakeLog log,
            IDotNetToolContentResolver contentResolver,
            ICakeConfiguration config,
            IFileSystem fileSystem)
        {
            m_Implementation = new DotNetToolPackageInstaller(environment, processRunner, log, contentResolver, config, fileSystem);
        }

        public IReadOnlyCollection<IFile> Install(PackageReference package, PackageType type, DirectoryPath path) =>
            m_Implementation.Install(package, type, path);
    }

}
