using System;
using Cake.Common.Build;
using Cake.Common.Build.AzurePipelines.Data;
using Cake.Core.Diagnostics;

namespace Build
{
    public class GitHubPullRequestContext
    {
        private readonly BuildContext m_Context;


        public bool IsPullRequest { get; }

        public int Number { get; }


        public GitHubPullRequestContext(BuildContext context)
        {
            //TODO: Can this also be determined when building locally (or on a CI system other than Azure Pipelines)
            if (context.AzurePipelines.IsActive)
            {
                IsPullRequest =
                    context.AzurePipelines().Environment.Repository.Provider == AzurePipelinesRepositoryType.GitHub &&
                    context.AzurePipelines().Environment.PullRequest.IsPullRequest;

                Number = context.AzurePipelines().Environment.PullRequest.Number;
            }
            else
            {
                IsPullRequest = false;
                Number = 0;
            }

            m_Context = context ?? throw new ArgumentNullException(nameof(context));
        }



        public void PrintToLog(int indentWidth = 0)
        {
            string prefix = new String(' ', indentWidth);

            m_Context.Log.Information($"{prefix}{nameof(IsPullRequest)}: {IsPullRequest}");
            m_Context.Log.Information($"{prefix}{nameof(Number)}: {Number}");
        }

    }
}
