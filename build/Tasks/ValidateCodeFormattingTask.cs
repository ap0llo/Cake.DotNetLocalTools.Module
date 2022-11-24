using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Format;
using Cake.Core;
using Cake.Frosting;

namespace Build.Tasks
{
    [TaskName("ValidateCodeFormatting")]
    [TaskDescription("Validates that all code files are formatted properly.")]
    [IsDependeeOf(typeof(ValidateTask))]
    public class ValidateCodeFormattingTask : FrostingTask<BuildContext>
    {
        public override bool ShouldRun(BuildContext context) => context.CodeFormattingSettings.EnableAutomaticFormatting;

        public override void Run(BuildContext context)
        {
            var settings = new DotNetFormatSettings()
            {
                VerifyNoChanges = true,
                NoRestore = true
            };

            // "dotnet format" expects the excluded directories to be passed as realtive paths (relative to the process' working directory)
            // To ensure that, start "dotnet format" in the repository root directory and convert all exclude paths to relative paths.
            // Since the DotNetFormat() alias does not seem to handle this out-of-the-box, use ArgumentCustomization to add the --exclude parameter
            settings.WorkingDirectory = context.RootDirectory;
            if (context.CodeFormattingSettings.ExcludedDirectories?.Count > 0)
            {
                settings.ArgumentCustomization = args =>
                {
                    args.Append("--exclude");
                    foreach (var excludedDirectory in context.CodeFormattingSettings.ExcludedDirectories)
                    {
                        var absolutePath = excludedDirectory.MakeAbsolute(context.Environment.WorkingDirectory);
                        var relativePath = settings.WorkingDirectory.GetRelativePath(absolutePath);
                        args.AppendQuoted(relativePath.ToString());
                    }
                    return args;
                };

            }

            context.DotNetFormat(context.SolutionPath.FullPath, settings);
        }
    }
}
