using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;

namespace Build.Tools.ChangeLog
{

    internal class ChangeLogRunner : Tool<ChangeLogSettings>
    {
        public ChangeLogRunner(IFileSystem fileSystem, ICakeEnvironment environment, IProcessRunner processRunner, IToolLocator tools) : base(fileSystem, environment, processRunner, tools)
        { }


        public void Run(ChangeLogSettings settings)
        {
            if (settings is null)
                throw new ArgumentNullException(nameof(settings));

            Run(settings, GetArguments(settings));
        }


        protected override IEnumerable<string> GetToolExecutableNames()
        {
            return new[] { "changelog", "changelog.exe" };
        }

        protected override string GetToolName() => "changelog";

        private ProcessArgumentBuilder GetArguments(ChangeLogSettings settings)
        {
            var builder = new ProcessArgumentBuilder();

            if (settings.RepositoryPath is not null)
            {
                builder.Append("--repository");
                builder.AppendQuoted(settings.RepositoryPath.FullPath);
            }

            if (!String.IsNullOrEmpty(settings.CurrentVersion))
            {
                builder.Append("--currentVersion");
                builder.AppendQuoted(settings.CurrentVersion);
            }

            if (!String.IsNullOrEmpty(settings.VersionRange))
            {
                builder.Append("--versionRange");
                builder.AppendQuoted(settings.VersionRange);
            }

            if (settings.OutputPath is not null)
            {
                builder.Append("--outputpath");
                builder.AppendQuoted(settings.OutputPath.FullPath);
            }

            if (settings.Template.HasValue)
            {
                builder.Append("--template");
                builder.AppendQuoted(settings.Template.Value.ToString());
            }

            if (settings.Verbose)
            {
                builder.Append("--verbose");
            }

            if (settings.IntegrationProvider.HasValue)
            {
                builder.Append("--integrationProvider");
                builder.AppendQuoted(settings.IntegrationProvider.Value.ToString());
            }

            return builder;
        }
    }
}
