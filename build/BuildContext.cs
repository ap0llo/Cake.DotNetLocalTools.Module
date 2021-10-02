using System;
using Cake.Common;
using Cake.Common.Tools.DotNetCore.MSBuild;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;

namespace Build
{
    public class BuildContext : FrostingContext
    {
        public class ArtifactNameSettings
        {
            /// <summary>
            /// The name of the main artifact on Azure Pipelines
            /// </summary>
            public string Binaries => "Binaries";

            /// <summary>
            /// The Azure Pipelines artifact name under which to save test result files
            /// </summary>
            public string TestResults => "TestResults";
        }

        /// <summary>
        /// Gets the names of the Artifacts to publish (when running in Azure DevOps)
        /// </summary>
        public ArtifactNameSettings ArtifactNames { get; } = new();


        /// <summary>
        /// Gets the configuration to build (Debug/Relesae)
        /// </summary>
        public string BuildConfiguration { get; set; }

        /// <summary>
        /// Gets the root directory of the current repository
        /// </summary>
        public DirectoryPath RootDirectory { get; }

        /// <summary>
        /// Gets the path of the Visual Studio Solution to build
        /// </summary>
        public FilePath SolutionPath => RootDirectory.CombineWithFilePath("Cake.DotNetLocalTools.Module.sln");

        /// <summary>
        /// Gets whether the current build is running in a CI environment
        /// </summary>
        public bool IsRunningInCI => this.IsRunningOnAzurePipelines();

        /// <summary>
        /// Gets the root output directory
        /// </summary>
        public DirectoryPath BinariesDirectory { get; }

        /// <summary>
        /// Gets the output directory for test results
        /// </summary>
        public DirectoryPath TestResultsPath { get; }

        /// <summary>
        /// Gets the output path for NuGet packages
        /// </summary>
        public DirectoryPath PackageOutputPath { get; }

        /// <summary>
        /// Determines whether to use deterministic build settigns
        /// </summary>
        public bool DeterministicBuild { get; }



        public BuildContext(ICakeContext context) : base(context)
        {
            RootDirectory = context.Environment.WorkingDirectory;

            var binariesDirectory = context.EnvironmentVariable("BUILD_BINARIESDIRECTORY");
            BinariesDirectory = String.IsNullOrEmpty(binariesDirectory) ? RootDirectory.Combine("Binaries") : binariesDirectory;

            BuildConfiguration = context.Argument("configuration", "Release");
            DeterministicBuild = context.Argument("deterministic", IsRunningInCI);

            PackageOutputPath = BinariesDirectory.Combine(BuildConfiguration).Combine("packages");
            TestResultsPath = BinariesDirectory.Combine(BuildConfiguration).Combine("TestResults");
        }


        public DotNetCoreMSBuildSettings GetDefaultMSBuildSettings() => new()
        {
            TreatAllWarningsAs = MSBuildTreatAllWarningsAs.Error
        };

    }
}
