using System;
using System.Linq;
using System.Xml.Linq;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Mono.Cecil;

namespace Build
{
    internal class TestRunNamer
    {
        private static readonly XNamespace s_TrxNamespace = XNamespace.Get("http://microsoft.com/schemas/VisualStudio/TeamTest/2010");

        private readonly ICakeLog m_Log;
        private readonly ICakeEnvironment m_Environment;
        private readonly IFileSystem m_FileSystem;


        public TestRunNamer(ICakeLog log, ICakeEnvironment environment, IFileSystem fileSystem)
        {
            m_Log = log ?? throw new ArgumentNullException(nameof(log));
            m_Environment = environment ?? throw new ArgumentNullException(nameof(environment));
            m_FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }


        public string GetTestRunName(FilePath testResultPath)
        {
            // Default case: Use the name of the test result file
            var name = testResultPath.GetFilenameWithoutExtension().ToString();

            try
            {
                // For supported formats, try to get a better test run name
                switch (testResultPath.GetExtension().ToLower())
                {
                    case ".trx":
                        var trxTestRunName = GetTrxTestRunName(testResultPath);
                        if (!String.IsNullOrWhiteSpace(trxTestRunName))
                        {
                            name = trxTestRunName;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                m_Log.Warning($"Failed to determine test run name for file '{testResultPath}': {ex.Message}. Falling back to use the file name.");
                name = testResultPath.GetFilenameWithoutExtension().ToString();
            }

            return name.ToString();
        }

        private string? GetTrxTestRunName(FilePath testResultPath)
        {
            var document = XDocument.Load(testResultPath.MakeAbsolute(m_Environment).FullPath);

            if (document.Root is null)
                return null;

            var assemblyFiles = document.Root
                // Get the assembly path for all tests in the TRX file
                .Elements(s_TrxNamespace.GetName("TestDefinitions"))
                .Elements(s_TrxNamespace.GetName("UnitTest"))
                .Elements(s_TrxNamespace.GetName("TestMethod"))
                .Select(x => x.Attribute("codeBase")?.Value)
                .Where(x => !String.IsNullOrEmpty(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                // Filter down to files that exist on disk
                .Where(x => m_FileSystem.Exist(new FilePath(x)))
                .Select(x => m_FileSystem.GetFile(new FilePath(x)))
                .ToHashSet();

            // no assemblies found => cannot determine test run name
            if (assemblyFiles.Count == 0)
                return null;

            // For the test run name, use the assembly name + the assembly's target framework
            // If there are multiple values, join them together using " | "
            return String.Join(" | ", assemblyFiles.Select(assemblyFile =>
            {
                var name = assemblyFile.Path.GetFilename().ToString();
                var targetFramework = GetTargetFrameworkFromAssembly(assemblyFile.Path.FullPath);

                return String.IsNullOrEmpty(targetFramework)
                    ? name
                    : $"{name} ({targetFramework})";
            }));
        }

        private static string? GetTargetFrameworkFromAssembly(string path)
        {
            using var assemblyDefinition = AssemblyDefinition.ReadAssembly(path);

            // Read the assembly's TargetFramework attribute
            var targetFrameworkAttribute = assemblyDefinition.CustomAttributes.SingleOrDefault(x => x.AttributeType.FullName == "System.Runtime.Versioning.TargetFrameworkAttribute");

            if (targetFrameworkAttribute is null)
                return null;

            return (string)targetFrameworkAttribute.ConstructorArguments.Single().Value;
        }

    }
}
