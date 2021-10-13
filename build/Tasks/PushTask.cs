using System;
using System.Collections.Generic;
using System.Linq;
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
            return context.IsRunningInCI && context.PushTargets.Any(x => x.IsActive(context));
        }

        public override void Run(BuildContext context)
        {
            var activePushTargets = context.PushTargets.Where(x => x.IsActive(context));
            foreach (var target in activePushTargets)
            {
                switch (target.Type)
                {
                    case PushTargetType.AzureArtifacts:
                        PushToAzureArtifacts(context, target);
                        break;

                    case PushTargetType.NuGetOrg:
                        PushToNuGetOrg(context, target);
                        break;

                    default:
                        throw new NotImplementedException($"Unimplemented push target type '{target.Type}'");
                }
            }
        }


        private void PushToAzureArtifacts(BuildContext context, PushTarget pushTarget)
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
                    Source = pushTarget.FeedUrl,
                    UserName = "AzureArtifacts",
                    Password = accessToken
                });

            context.Log.Information($"Pushing packages to Azure Artifacts feed '{pushTarget.FeedUrl}'");
            foreach (var package in context.Output.PackageFiles)
            {
                context.Log.Information($"Pushing package '{package}'");
                var pushSettings = new DotNetCoreNuGetPushSettings()
                {
                    Source = "AzureArtifacts",
                    ApiKey = "AzureArtifacts"
                };

                context.DotNetCoreNuGetPush(package.FullPath, pushSettings);
            }
        }

        private void PushToNuGetOrg(BuildContext context, PushTarget pushTarget)
        {
            var apiKey = context.EnvironmentVariable("NUGET_ORG_APIKEY");
            if (String.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("Could not determine nuget.org API key. Enviornment variable 'NUGET_ORG_APIKEY' is empty.");
            }

            context.Log.Information($"Pushing packages to nuget.org (feed {pushTarget.FeedUrl})");
            foreach (var package in context.Output.PackageFiles)
            {
                context.Log.Information($"Pushing package '{package}'");
                var pushSettings = new DotNetCoreNuGetPushSettings()
                {
                    Source = pushTarget.FeedUrl,
                    ApiKey = apiKey
                };

                context.DotNetCoreNuGetPush(package.FullPath, pushSettings);
            }
        }
    }
}
