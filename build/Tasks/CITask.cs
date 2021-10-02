using Cake.Frosting;

namespace Build.Tasks
{
    [TaskName("CI")]
    [TaskDescription("Main entry point for the Continuous Integration Build")]
    [IsDependentOn(typeof(SetBuildNumberTask))]
    [IsDependentOn(typeof(BuildTask))]
    [IsDependentOn(typeof(TestTask))]
    [IsDependentOn(typeof(PackTask))]
    public class CITask : FrostingTask
    { }
}
