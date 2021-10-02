using System;
using Cake.Common;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.NuGet.Push;
using Cake.Common.Tools.DotNetCore.NuGet.Source;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;

namespace Build.Tasks
{
    [TaskName("Push")]
    [IsDependentOn(typeof(PackTask))]
    public class PushTask : FrostingTask<BuildContext>
    {
        public override bool ShouldRun(BuildContext context)
        {
            return context.IsRunningInCI && (context.IsMasterBranch || context.IsReleaseBranch);
        }

        public override void Run(BuildContext context)
        {
            var packages = context.FileSystem.GetFilePaths(context.PackageOutputPath, "*.nupkg", SearchScope.Current);
            context.Log.Information($"Found {packages.Count} packages in the package output directory '{context.PackageOutputPath}'");

            ConfigureNuGetCredentials(context);

            //
            // NuGet push
            //
            foreach (var package in packages)
            {
                var pushSettings = new DotNetCoreNuGetPushSettings()
                {
                    Source = "AzureArtifacts",
                    ApiKey = "AzureArtifacts"
                };

                context.DotNetCoreNuGetPush(package.FullPath, pushSettings);
            }
        }


        private void ConfigureNuGetCredentials(BuildContext context)
        {
            // See https://www.daveaglick.com/posts/pushing-packages-from-azure-pipelines-to-azure-artifacts-using-cake
            var accessToken = context.EnvironmentVariable("SYSTEM_ACCESSTOKEN");
            if (String.IsNullOrEmpty(accessToken))
            {
                throw new InvalidOperationException("Could not resolve SYSTEM_ACCESSTOKEN.");
            }

            context.DotNetCoreNuGetAddSource(
                "AzureArtifacts",
                new DotNetCoreNuGetSourceSettings()
                {
                    Source = context.CINuGetFeedUrl,
                    UserName = "AzureArtifacts",
                    Password = accessToken
                });
        }
    }
}
