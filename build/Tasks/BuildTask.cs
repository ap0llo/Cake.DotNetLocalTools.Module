using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Build;
using Cake.Common.Tools.DotNet.MSBuild;
using Cake.Common.Tools.DotNet.Restore;
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
            context.DotNetRestore(context.SolutionPath.FullPath, new DotNetRestoreSettings()
            {
                MSBuildSettings = context.BuildSettings.GetDefaultMSBuildSettings()
            });

            //
            // Build
            //
            context.Log.Information($"Building {context.SolutionPath}");
            var buildSettings = new DotNetBuildSettings()
            {
                Configuration = context.BuildSettings.Configuration,
                NoRestore = true,
                MSBuildSettings = context.BuildSettings.GetDefaultMSBuildSettings()
            };

            if (context.BuildSettings.Deterministic)
            {
                context.Log.Information("Using deterministic build settings");
                buildSettings.MSBuildSettings.WithProperty("ContinuousIntegrationBuild", "true");
                buildSettings.MSBuildSettings.WithProperty("Deterministic", "true");
            }

            context.DotNetBuild(context.SolutionPath.FullPath, buildSettings);
        }
    }
}
