using Build.Tools.ChangeLog;
using Cake.Common;
using Cake.Common.Build;
using Cake.Core.Diagnostics;
using Cake.Frosting;
using Cake.GitVersioning;

namespace Build.Tasks
{
    [TaskName("GenerateChangeLog")]
    public class GenerateChangeLogTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            //
            // Generate change log
            //
            context.Log.Information("Generating changelog");
            var versionInfo = context.GitVersioningGetVersion(context.RootDirectory.FullPath);
            var version = versionInfo.NuGetPackageVersion;

            var changelogSettings = new ChangeLogSettings()
            {
                RepositoryPath = context.RootDirectory,
                CurrentVersion = version,
                VersionRange = $"[{version}]",
                OutputPath = context.ChangeLogOutputPath,
                Template = ChangeLogTemplate.GitHubRelease,
                Verbose = true
            };

            if (context.EnvironmentVariable("GITHUB_ACCESSTOKEN") is string { Length: > 0 } accessToken)
            {
                context.Log.Information("GitHub access token specified, activating changelog's GitHub integration");
                changelogSettings.IntegrationProvider = ChangeLogIntegrationProvider.GitHub;
                changelogSettings.EnvironmentVariables["CHANGELOG__INTEGRATIONS__GITHUB__ACCESSTOKEN"] = accessToken;
            }
            else
            {
                context.Log.Warning("No GitHub access token specified, generating change log without GitHub integration");
            }

            context.ChangeLog(changelogSettings);

            //
            // Publish changelog as pipeline artifact
            //
            if (context.IsRunningOnAzurePipelines())
            {
                context.Log.Information("Publishing change log to Azure Pipelines");
                context.AzurePipelines().Commands.UploadArtifact("", context.ChangeLogOutputPath, context.ArtifactNames.ChangeLog);
            }
        }
    }
}
