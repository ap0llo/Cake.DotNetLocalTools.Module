using Cake.Frosting;

namespace Build.Tasks
{
    [TaskName("PrintBuildContext")]
    public class PrintBuildContextTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context) => context.PrintToLog();
    }
}
