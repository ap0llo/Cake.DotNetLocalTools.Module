using System;
using System.Collections.Generic;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;

namespace Build
{
    public class BuildContext : FrostingContext
    {
        /// <summary>
        /// Gets whether the current build is running in a CI environment
        /// </summary>
        public bool IsRunningInCI => AzurePipelines.IsActive;

        public AzurePipelinesContext AzurePipelines { get; }

        public GitContext Git { get; }

        public GitHubContext GitHub { get; }

        public IReadOnlyCollection<PushTarget> PushTargets { get; }

        public OutputContext Output { get; }

        public BuildSettings BuildSettings { get; }

        /// <summary>
        /// Gets the root directory of the current repository
        /// </summary>
        public DirectoryPath RootDirectory { get; }

        /// <summary>
        /// Gets the path of the Visual Studio Solution to build
        /// </summary>
        public FilePath SolutionPath => RootDirectory.CombineWithFilePath("Cake.DotNetLocalTools.Module.sln");

        public virtual CodeFormattingSettings CodeFormattingSettings { get; }

        public virtual TestSettings TestSettings { get; }


        public BuildContext(ICakeContext context) : base(context)
        {
            RootDirectory = context.Environment.WorkingDirectory;

            PushTargets = new[]
            {
                new PushTarget(
                    this,
                    PushTargetType.AzureArtifacts,
                    "https://pkgs.dev.azure.com/ap0llo/OSS/_packaging/PublicCI/nuget/v3/index.json",
                    context => context.Git.IsMasterBranch || context.Git.IsReleaseBranch
                ),
                new PushTarget(
                    this,
                    PushTargetType.NuGetOrg,
                    "https://api.nuget.org/v3/index.json",
                    context => context.Git.IsReleaseBranch
                ),
            };


            AzurePipelines = new(this);
            Git = new(this);
            GitHub = new(this);
            Output = new(this);
            BuildSettings = new(this);
            CodeFormattingSettings = new(this)
            {
                // Exclude the "deps" directory from code formatting (contains submodules for which this project's formatting rules do not apply)
                ExcludedDirectories = new[]
                {
                    RootDirectory.Combine("deps")
                }
            };
            TestSettings = new(this);
        }


        public void PrintToLog(int indentWidth = 0)
        {
            static string prefix(int width) => new String(' ', width);

            Log.Information($"{prefix(indentWidth)}{nameof(IsRunningInCI)}: {IsRunningInCI}");

            Log.Information($"{prefix(indentWidth)}{nameof(RootDirectory)}: {RootDirectory.FullPath}");

            Log.Information($"{prefix(indentWidth)}{nameof(SolutionPath)}: {SolutionPath.FullPath}");

            Log.Information($"{prefix(indentWidth)}{nameof(Output)}:");
            Output.PrintToLog(indentWidth + 2);

            Log.Information($"{prefix(indentWidth)}{nameof(BuildSettings)}:");
            BuildSettings.PrintToLog(indentWidth + 2);

            Log.Information($"{prefix(indentWidth)}{nameof(Git)}:");
            Git.PrintToLog(indentWidth + 2);

            Log.Information($"{prefix(indentWidth)}{nameof(AzurePipelines)}:");
            AzurePipelines.PrintToLog(indentWidth + 2);

            Log.Information($"{prefix(indentWidth)}{nameof(GitHub)}:");
            GitHub.PrintToLog(indentWidth + 2);

            Log.Information($"{prefix(indentWidth)}{nameof(CodeFormattingSettings)}:");
            CodeFormattingSettings.PrintToLog(indentWidth + 2);

            Log.Information($"{prefix(indentWidth)}{nameof(TestSettings)}:");
            TestSettings.PrintToLog(indentWidth + 2);

            // 
            Log.Information($"{nameof(PushTargets)}:");
            int index = 0;
            foreach (var pushTarget in PushTargets)
            {
                Log.Information($"{prefix(indentWidth + 2)}{nameof(PushTargets)}[{index}]:");
                pushTarget.PrintToLog(indentWidth + 4);
                index++;
            }
        }
    }
}
