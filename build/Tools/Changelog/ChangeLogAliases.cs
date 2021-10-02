using System;
using Cake.Core;

namespace Build.Tools.ChangeLog
{
    internal static class ChangeLogAliases
    {
        public static void ChangeLog(this ICakeContext context, ChangeLogSettings settings)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            if (settings is null)
                throw new ArgumentNullException(nameof(settings));

            var runner = new ChangeLogRunner(context.FileSystem, context.Environment, context.ProcessRunner, context.Tools);
            runner.Run(settings);
        }
    }
}
