using System;
using System.IO;
using System.Threading.Tasks;
using CliWrap;
using Grynwald.Utilities.IO;
using Xunit;
using Xunit.Abstractions;

namespace Cake.LocalTools.Module.Test
{
    public class E2ETests
    {
        private readonly ITestOutputHelper m_TestOutputHelper;


        public E2ETests(ITestOutputHelper testOutputHelper)
        {
            m_TestOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }


        [Fact]
        public async Task Local_tools_can_be_installed_into_a_Cake_Frosting_project()
        {
            // ARRANGE
            using var temporaryDirectory = new TemporaryDirectory();
            temporaryDirectory.AddFile(
                "build.csproj",
                $@"<Project Sdk=""Microsoft.NET.Sdk"">
  
                    <PropertyGroup>
                        <OutputType>Exe</OutputType>
                        <TargetFramework>net5.0</TargetFramework>
                        <RunWorkingDirectory>$(MSBuildProjectDirectory)</RunWorkingDirectory>
                        <IsPackable>false</IsPackable>
                    </PropertyGroup>
    
                    <ItemGroup>
                        <PackageReference Include=""Cake.Frosting"" Version=""1.2.0"" />
                        <Reference Include=""{typeof(LocalToolsModule).Assembly.Location}"" />
                    </ItemGroup>

                </Project>");

            temporaryDirectory.AddFile("global.json", @"{
              ""sdk"": {
                ""version"": ""5.0.400""
              }
            }");

            temporaryDirectory.AddFile(
                "Program.cs",
                @"
                    using Cake.Core;
                    using Cake.Frosting;
                    using Cake.LocalTools.Module;

                    return new CakeHost()
                        .UseModule<LocalToolsModule>()
                        .UseContext<BuildContext>()
                        .InstallToolsFromManifest("".config/dotnet-tools.json"")
                        .Run(args);

                    public class BuildContext : FrostingContext
                    {
                        public BuildContext(ICakeContext context) : base(context)
                        { }
                    }

                    [TaskName(""Default"")]
                    public class DefaultTask : FrostingTask
                    {
                    }
            ");

            temporaryDirectory.AddFile(".config/dotnet-tools.json", @"{
              ""version"": 1,
              ""isRoot"": true,
              ""tools"": {
                ""nbgv"": {
                  ""version"": ""3.4.231"",
                  ""commands"": [
                    ""nbgv""
                  ]
                },
                ""dotnet-format"": {
                  ""version"": ""5.1.225507"",
                  ""commands"": [
                    ""dotnet-format""
                  ]
                },
                ""dotnet-reportgenerator-globaltool"": {
                  ""version"": ""4.8.12"",
                  ""commands"": [
                    ""reportgenerator""
                  ]
                }
              }
            }");

            temporaryDirectory.AddFile("nuget.config", @"<?xml version=""1.0"" encoding=""utf-8""?>
                <configuration>
                  <packageSources>
                    <clear />
                    <add key=""nuget"" value=""https://api.nuget.org/v3/index.json"" />
                  </packageSources>
                </configuration>
            ");

            var command = Cli.Wrap("dotnet")
                .WithArguments(args => args
                    .Add("run")
                    .Add("--")
                    .Add("--verbosity").Add("verbose")
                )
                .WithStandardErrorPipe(PipeTarget.ToDelegate(m_TestOutputHelper.WriteLine))
                .WithStandardOutputPipe(PipeTarget.ToDelegate(m_TestOutputHelper.WriteLine))
                .WithWorkingDirectory(temporaryDirectory)
                .WithValidation(CommandResultValidation.ZeroExitCode);

            // ACT 
            var result = await command.ExecuteAsync();

            // ASSERT
            Assert.True(File.Exists(Path.Combine(temporaryDirectory, "tools", "nbgv.exe")));
            Assert.True(File.Exists(Path.Combine(temporaryDirectory, "tools", "dotnet-format.exe")));
            Assert.True(File.Exists(Path.Combine(temporaryDirectory, "tools", "reportgenerator.exe")));
        }
    }
}
