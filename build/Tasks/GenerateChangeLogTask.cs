using Build.Tools.ChangeLog;
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
                OutputPath = context.Output.ChangeLogFile,
                Template = ChangeLogTemplate.GitHubRelease,
                Verbose = true
            };

            if (context.GitHub.TryGetAccessToken() is string accessToken)
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
            if (context.AzurePipelines.IsActive)
            {
                context.Log.Information("Publishing change log to Azure Pipelines");
                context.AzurePipelines().Commands.UploadArtifact("", context.Output.ChangeLogFile, context.AzurePipelines.ArtifactNames.ChangeLog);
            }
        }
    }
}
