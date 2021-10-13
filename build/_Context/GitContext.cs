using System;
using Cake.Common.Build;
using Cake.Core.Diagnostics;
using Cake.Core.IO;

namespace Build
{
    public class GitContext
    {
        private readonly BuildContext m_Context;


        public string BranchName
        {
            get
            {
                if (m_Context.AzurePipelines.IsActive)
                {
                    var branchName = m_Context.AzurePipelines().Environment.Repository.SourceBranch;

                    if (branchName.StartsWith("refs/heads/"))
                    {
                        branchName = branchName["refs/heads/".Length..];
                    }

                    return branchName;
                }
                else
                {
                    return StartGit("rev-parse", "--abbrev-ref", "HEAD").Trim();
                }

            }
        }

        public string CommitId => m_Context.AzurePipelines.IsActive
            ? m_Context.AzurePipelines().Environment.Repository.SourceVersion
            : StartGit("rev-parse", "HEAD").Trim();

        public string RemoteUrl => StartGit("remote", "get-url", "origin").Trim();

        public bool IsMasterBranch => BranchName.Equals("master", StringComparison.OrdinalIgnoreCase);

        public bool IsReleaseBranch => BranchName.StartsWith("release/", StringComparison.OrdinalIgnoreCase);


        public GitContext(BuildContext context)
        {
            m_Context = context ?? throw new ArgumentNullException(nameof(context));

        }


        public void PrintToLog(int indentWidth = 0)
        {
            string prefix = new String(' ', indentWidth);

            m_Context.Log.Information($"{prefix}{nameof(BranchName)}: {BranchName}");
            m_Context.Log.Information($"{prefix}{nameof(CommitId)}: {CommitId}");
            m_Context.Log.Information($"{prefix}{nameof(RemoteUrl)}: {RemoteUrl}");
            m_Context.Log.Information($"{prefix}{nameof(IsMasterBranch)}: {IsMasterBranch}");
            m_Context.Log.Information($"{prefix}{nameof(IsReleaseBranch)}: {IsReleaseBranch}");
        }


        private string StartGit(params string[] args)
        {
            var process = m_Context.ProcessRunner.Start("git", new ProcessSettings()
            {
                Arguments = ProcessArgumentBuilder.FromStrings(args),
                RedirectStandardOutput = true
            });

            process.WaitForExit();

            var exitCode = process.GetExitCode();
            if (exitCode != 0)
                throw new Exception($"Command 'git {String.Join(" ", args)}' completed with exit code {exitCode}");

            var stdOut = String.Join(Environment.NewLine, process.GetStandardOutput());
            return stdOut;
        }
    }
}
