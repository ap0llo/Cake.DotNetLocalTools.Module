using System.Threading.Tasks;
using CliWrap;
using Xunit.Abstractions;

namespace Cake.LocalTools.Module.Test
{
    internal static class CommandExtensions
    {
        public static async Task<CommandResult> ExecuteWithTestOutputAsync(this Command command, ITestOutputHelper testOutputHelper)
        {
            return await command
                .WithStandardErrorPipe(PipeTarget.ToDelegate(testOutputHelper.WriteLine))
                .WithStandardOutputPipe(PipeTarget.ToDelegate(testOutputHelper.WriteLine))
                .ExecuteAsync();
        }
    }
}
