using System;
using Cake.Common;
using Cake.Core.Diagnostics;
using Grynwald.ChangeLog.Integrations.GitHub;

namespace Build
{
    public class GitHubContext
    {
        private readonly BuildContext m_Context;
        private readonly Lazy<GitHubProjectInfo> m_ProjectInfo;


        public string HostName => m_ProjectInfo.Value.Host;

        public string RepositoryOwner => m_ProjectInfo.Value.Owner;

        public string RepositoryName => m_ProjectInfo.Value.Repository;


        public GitHubContext(BuildContext context)
        {
            m_Context = context ?? throw new ArgumentNullException(nameof(context));
            m_ProjectInfo = new Lazy<GitHubProjectInfo>(() => GitHubUrlParser.ParseRemoteUrl(m_Context.Git.RemoteUrl));
        }


        public void PrintToLog(int indentWidth = 0)
        {
            string prefix = new String(' ', indentWidth);

            m_Context.Log.Information($"{prefix}{nameof(HostName)}: {HostName}");
            m_Context.Log.Information($"{prefix}{nameof(RepositoryOwner)}: {RepositoryOwner}");
            m_Context.Log.Information($"{prefix}{nameof(RepositoryName)}: {RepositoryName}");
        }

        public string? TryGetAccessToken()
        {
            if (m_Context.EnvironmentVariable("GITHUB_ACCESSTOKEN") is string { Length: > 0 } accessToken)
            {
                return accessToken;
            }
            else
            {
                return null;
            }
        }
    }
}
