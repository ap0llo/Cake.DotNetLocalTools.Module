using System;
using System.Threading.Tasks;
using Cake.Core.Diagnostics;
using Cake.Frosting;
using Cake.GitHub;
using Cake.GitVersioning;

namespace Build.Tasks
{
    [TaskName("SetGitHubMilestone")]
    public class SetGitHubMilestoneTask : AsyncFrostingTask<BuildContext>
    {
        public override bool ShouldRun(BuildContext context)
        {
            return context.IsRunningInCI && context.GitHub.PullRequest.IsPullRequest;
        }


        public override async Task RunAsync(BuildContext context)
        {
            var versionInfo = context.GitVersioningGetVersion(context.RootDirectory.FullPath);
            var milestoneTitle = $"v{versionInfo.Version.Major}.{versionInfo.Version.Minor}";

            context.Log.Information($"Assinging Pull Request {context.GitHub.PullRequest.Number} to milestone {milestoneTitle}");

            if (context.GitHub.TryGetAccessToken() is not string accessToken)
            {
                throw new Exception("No GitHub access token specified. Cannot set Milestone for Pull Request");
            }

            await context.GitHubSetMilestoneAsync(
                userName: null,
                accessToken,
                context.GitHub.RepositoryOwner,
                context.GitHub.RepositoryName,
                context.GitHub.PullRequest.Number,
                milestoneTitle,
                new GitHubSetMilestoneSettings()
                {
                    Overwrite = true,
                    CreateMilestone = true
                });
        }

    }
}
