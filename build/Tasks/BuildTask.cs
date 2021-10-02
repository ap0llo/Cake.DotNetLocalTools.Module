using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Build;
using Cake.Common.Tools.DotNetCore.MSBuild;
using Cake.Common.Tools.DotNetCore.Restore;
using Cake.Core.Diagnostics;
using Cake.Frosting;

namespace Build.Tasks
{
    [TaskName("Build")]
    public class BuildTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            //
            // Restore NuGet Packages
            //
            context.Log.Information("Restoring NuGet Packages");
            context.DotNetCoreRestore(context.SolutionPath.FullPath, new DotNetCoreRestoreSettings()
            {
                MSBuildSettings = context.GetDefaultMSBuildSettings()
            });

            //
            // Build
            //
            context.Log.Information($"Building {context.SolutionPath}");
            var buildSettings = new DotNetCoreBuildSettings()
            {
                Configuration = context.BuildConfiguration,
                NoRestore = true,
                MSBuildSettings = context.GetDefaultMSBuildSettings()
            };

            if (context.DeterministicBuild)
            {
                context.Log.Information("Using deterministic build settings");
                buildSettings.MSBuildSettings.WithProperty("ContinuousIntegrationBuild", "true");
                buildSettings.MSBuildSettings.WithProperty("Deterministic", "true");
            }

            context.DotNetCoreBuild(context.SolutionPath.FullPath, buildSettings);
        }
    }
}
