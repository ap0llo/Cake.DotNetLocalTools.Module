using Cake.Frosting;

namespace Build.Tasks
{
    [TaskName("CI")]
    [TaskDescription("Main entry point for the Continuous Integration Build")]
    [IsDependentOn(typeof(PrintBuildContextTask))]
    [IsDependentOn(typeof(SetBuildNumberTask))]
    [IsDependentOn(typeof(BuildTask))]
    [IsDependentOn(typeof(TestTask))]
    [IsDependentOn(typeof(PackTask))]
    [IsDependentOn(typeof(GenerateChangeLogTask))]
    [IsDependentOn(typeof(PushTask))]
    [IsDependentOn(typeof(CreateGitHubReleaseTask))]
    public class CITask : FrostingTask
    { }
}
