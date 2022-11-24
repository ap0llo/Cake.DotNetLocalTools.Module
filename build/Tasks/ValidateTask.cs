using Cake.Frosting;

namespace Build.Tasks
{
    [TaskName("Validate")]
    [TaskDescription("Validates all files under source control")]
    public class ValidateTask : FrostingTask
    {
    }
}
