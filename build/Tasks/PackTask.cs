using Cake.Common.Build;
using Cake.Common.IO;
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
            context.EnsureDirectoryDoesNotExist(context.Output.PackagesDirectory);

            // 
            // Pack NuGet packages
            // 
            context.Log.Information("Packing NuGet Packages");
            var packSettings = new DotNetCorePackSettings()
            {
                Configuration = context.BuildSettings.Configuration,
                OutputDirectory = context.Output.PackagesDirectory,
                NoRestore = true,
                NoBuild = true,
                MSBuildSettings = context.BuildSettings.GetDefaultMSBuildSettings()
            };

            if (context.BuildSettings.Deterministic)
            {
                context.Log.Information("Using deterministic build settings");
                packSettings.MSBuildSettings.WithProperty("ContinuousIntegrationBuild", "true");
                packSettings.MSBuildSettings.WithProperty("Deterministic", "true");
            }

            context.DotNetCorePack(context.SolutionPath.FullPath, packSettings);

            //
            // Publish Artifacts
            //
            if (context.AzurePipelines.IsActive)
            {
                context.Log.Information("Publishing NuGet packages to Azure Pipelines");
                foreach (var file in context.Output.PackageFiles)
                {
                    context.Log.Debug("Publishing '{file}'");
                    context.AzurePipelines().Commands.UploadArtifact("", file, context.AzurePipelines.ArtifactNames.Binaries);
                }
            }
        }
    }
}
