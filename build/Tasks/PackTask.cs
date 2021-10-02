using Cake.Common.Build;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.MSBuild;
using Cake.Common.Tools.DotNetCore.Pack;
using Cake.Core.Diagnostics;
using Cake.Frosting;

namespace Build.Tasks
{
    [TaskName("Pack")]
    [IsDependentOn(typeof(BuildTask))]
    public class PackTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            //
            // Clean output directory
            //
            context.CleanDirectory(context.PackageOutputPath);

            // 
            // Pack NuGet packages
            // 
            context.Log.Information("Packing NuGet Packages");
            var packSettings = new DotNetCorePackSettings()
            {
                Configuration = context.BuildConfiguration,
                OutputDirectory = context.PackageOutputPath,
                NoRestore = true,
                NoBuild = true,
                MSBuildSettings = context.GetDefaultMSBuildSettings()
            };

            if (context.DeterministicBuild)
            {
                context.Log.Information("Using deterministic build settings");
                packSettings.MSBuildSettings.WithProperty("ContinuousIntegrationBuild", "true");
                packSettings.MSBuildSettings.WithProperty("Deterministic", "true");
            }

            context.DotNetCorePack(context.SolutionPath.FullPath, packSettings);

            //
            // Publish Artifacts
            //
            if (context.IsRunningOnAzurePipelines())
            {
                context.Log.Information("Publishing NuGet packages to Azure Pipelines");
                foreach (var file in context.FileSystem.GetFilePaths(context.PackageOutputPath, "*.nupkg"))
                {
                    context.Log.Debug("Publishing '{file}'");
                    context.AzurePipelines().Commands.UploadArtifact("", file, context.ArtifactNames.Binaries);
                }
            }
        }
    }
}
