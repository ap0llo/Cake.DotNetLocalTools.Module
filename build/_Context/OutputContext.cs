using System;
using System.Collections.Generic;
using Cake.Common;
using Cake.Core.Diagnostics;
using Cake.Core.IO;

namespace Build
{
    public class OutputContext
    {
        private readonly BuildContext m_Context;


        /// <summary>
        /// Gets the root output directory
        /// </summary>
        public DirectoryPath BinariesDirectory
        {
            get
            {
                var binariesDirectory = m_Context.EnvironmentVariable("BUILD_BINARIESDIRECTORY");
                return String.IsNullOrEmpty(binariesDirectory) ? m_Context.RootDirectory.Combine("Binaries") : binariesDirectory;
            }
        }

        /// <summary>
        /// Gets the output path for NuGet packages
        /// </summary>
        public DirectoryPath PackagesDirectory => BinariesDirectory.Combine(m_Context.BuildSettings.Configuration).Combine("packages");

        public DirectoryPath TestResultsDirectory => BinariesDirectory.Combine(m_Context.BuildSettings.Configuration).Combine("TestResults");

        public FilePath ChangeLogFile => BinariesDirectory.CombineWithFilePath("changelog.md");

        public IEnumerable<FilePath> PackageFiles => m_Context.FileSystem.GetFilePaths(PackagesDirectory, "*.nupkg");


        public OutputContext(BuildContext context)
        {
            m_Context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public void PrintToLog(int indentWidth = 0)
        {
            string prefix = new String(' ', indentWidth);

            m_Context.Log.Information($"{prefix}{nameof(BinariesDirectory)}: {BinariesDirectory.FullPath}");
            m_Context.Log.Information($"{prefix}{nameof(PackagesDirectory)}: {PackagesDirectory.FullPath}");
            m_Context.Log.Information($"{prefix}{nameof(TestResultsDirectory)}: {TestResultsDirectory.FullPath}");
            m_Context.Log.Information($"{prefix}{nameof(ChangeLogFile)}: {ChangeLogFile.FullPath}");
        }
    }
}
