using Cake.Core.IO;
using Cake.Core.Tooling;

namespace Build.Tools.ChangeLog
{
    internal class ChangeLogSettings : ToolSettings
    {
        public DirectoryPath? RepositoryPath { get; set; }

        public string? CurrentVersion { get; set; }

        public string? VersionRange { get; set; }

        public FilePath? OutputPath { get; set; }

        public ChangeLogTemplate? Template { get; set; }

        public bool Verbose { get; set; } = false;

        public ChangeLogIntegrationProvider? IntegrationProvider { get; set; }
    }
}
