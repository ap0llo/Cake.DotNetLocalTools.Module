using Cake.Common.Build;
using Cake.Common.IO;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;

namespace Build
{
    internal static class CakeContextExtensions
    {
        public static bool IsRunningOnAzurePipelines(this ICakeContext context)
        {
            return context.AzurePipelines().IsRunningOnAzurePipelines ||
                   context.AzurePipelines().IsRunningOnAzurePipelinesHosted;
        }

        public static void CleanDirectory(this ICakeContext context, DirectoryPath directory)
        {
            if (context.DirectoryExists(directory))
            {
                context.Log.Information($"Cleaning directory '{directory}'");
                context.DeleteDirectory(directory, new() { Force = true, Recursive = true });
            }
        }
    }
}
