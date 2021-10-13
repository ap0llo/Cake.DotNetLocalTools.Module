using System.IO;
using Cake.Core.IO;

namespace Build
{
    internal static class FileExtensions
    {
        public static string ReadAllText(this IFile file)
        {
            using var stream = file.OpenRead();
            using var streamReader = new StreamReader(stream);

            return streamReader.ReadToEnd();
        }

    }
}
