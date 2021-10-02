using Cake.Core.Diagnostics;
using Cake.Frosting;
using Cake.GitVersioning;

namespace Build.Tasks
{
    [TaskName("SetBuildNumber")]
    [TaskDescription("Sets the build number when running in a CI system")]
    public class SetBuildNumberTask : FrostingTask<BuildContext>
    {
        public override bool ShouldRun(BuildContext context) => context.IsRunningInCI;

        public override void Run(BuildContext context)
        {
            context.Log.Information("Setting Build Number using Nerdbank.GitVersioning");
            context.GitVersioningCloud(context.RootDirectory.FullPath, new() { AllVariables = true });
        }
    }
}
