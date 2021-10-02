using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Core.Packaging;

namespace Cake.LocalTools.Module
{
    /// <summary>
    /// Package installer for .NET Local Tool Manifests
    /// </summary>
    /// <remarks>
    /// Reads all package references from a .NET Local Tool manifest (<c>dotnet-tools.json</c>),
    /// converts them to a set of <c>dotnet:</c> package references and installs them 
    /// using the package installer for .NET CLI tools
    /// </remarks>
    internal class ToolManifestPackageInstaller : IPackageInstaller
    {
        public const string ToolManifestScheme = "toolmanifest";

        private readonly ICakeLog m_Log;
        private readonly IFileSystem m_FileSystem;
        private readonly IToolManifestReader m_ToolManifestReader;
        private readonly IDotNetToolPackageInstaller m_PackageInstallHelper;

        public ToolManifestPackageInstaller(ICakeLog log, IFileSystem fileSystem, IToolManifestReader toolManifestReader, IDotNetToolPackageInstaller packageInstallHelper)
        {
            m_Log = log ?? throw new ArgumentNullException(nameof(log));
            m_FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            m_ToolManifestReader = toolManifestReader ?? throw new ArgumentNullException(nameof(toolManifestReader));
            m_PackageInstallHelper = packageInstallHelper ?? throw new ArgumentNullException(nameof(packageInstallHelper));
        }


        public bool CanInstall(PackageReference package, PackageType type)
        {
            if (type != PackageType.Tool)
                return false;

            return package.Scheme.Equals(ToolManifestScheme, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public IReadOnlyCollection<IFile> Install(PackageReference package, PackageType type, DirectoryPath path)
        {
            if (package is null)
                throw new ArgumentNullException(nameof(package));

            if (type != PackageType.Tool)
                throw new InvalidOperationException($"{nameof(ToolManifestPackageInstaller)} cannot install packages of type '{type}'");

            if (!package.Scheme.Equals(ToolManifestScheme, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"{nameof(ToolManifestPackageInstaller)} cannot install package references with scheme '{package.Scheme}'");

            m_Log.Verbose($"Installing tools from local tool manifest");

            var manifestFile = GetToolManifestPath(package);

            var manifest = m_ToolManifestReader.Load(manifestFile);

            var allFiles = new List<IFile>();

            foreach (var entry in manifest.Entries)
            {
                m_Log.Verbose($"Installing tool {entry}");
                var currentPackageFiles = m_PackageInstallHelper.Install(entry.ToPackageReference(), type, path);
                allFiles.AddRange(currentPackageFiles);
            }

            return allFiles;
        }


        private IFile GetToolManifestPath(PackageReference package)
        {
            if (!package.Parameters.TryGetValue("package", out var values))
                throw new ArgumentException("No tool manifest path specified, expected as parameter 'package'");

            if (values.Count != 1)
                throw new ArgumentException("Multiple values for parameter 'package' in tool manifest reference");

            var value = values.Single();

            if (String.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value of parameter 'package' is empty");

            var file = m_FileSystem.GetFile(value);

            if (!file.Exists)
                throw new FileNotFoundException($"Tool manifest file '{file.Path}' does not exist");

            return file;
        }
    }
}
