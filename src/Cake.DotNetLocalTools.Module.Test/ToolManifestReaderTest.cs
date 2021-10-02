using System;
using System.IO;
using System.Linq;
using Cake.Core;
using Cake.Core.IO;
using Grynwald.Utilities.IO;
using Moq;
using Newtonsoft.Json;
using Xunit;
using Path = System.IO.Path;

namespace Cake.DotNetLocalTools.Module.Test
{
    /// <summary>
    /// Tests for <see cref="ToolManifestReader"/>
    /// </summary>
    public class ToolManifestReaderTest
    {
        private readonly Mock<ICakeEnvironment> m_EnvironmentMock;


        public ToolManifestReaderTest()
        {
            m_EnvironmentMock = new Mock<ICakeEnvironment>(MockBehavior.Strict);
        }


        [Fact]
        public void Load_checks_file_for_null()
        {
            // ARRANGE
            var sut = new ToolManifestReader(m_EnvironmentMock.Object);

            // ACT 
            var ex = Record.Exception(() => sut.Load(null!));

            // ASSERT
            var argumentException = Assert.IsType<ArgumentNullException>(ex);
            Assert.Equal("file", argumentException.ParamName);
        }

        [Fact]
        public void Load_throws_FileNotFoundException_if_file_does_not_exist()
        {
            // ARRANGE
            var sut = new ToolManifestReader(m_EnvironmentMock.Object);
            var fileMock = GetFileMock(exists: false, path: "some-path");

            // ACT 
            var ex = Record.Exception(() => sut.Load(fileMock.Object));

            // ASSERT
            Assert.IsType<FileNotFoundException>(ex);
        }

        [Fact]
        public void Load_uses_injected_environment_to_resolve_manifest_file_path()
        {
            // ARRANGE
            using var temporaryDirectory = new TemporaryDirectory();
            var manifestFile = CreateToolManifest(temporaryDirectory, @"{ ""version"" : 1 }");
            var fileMock = GetFileMock(exists: true, path: manifestFile.Path.GetFilename().ToString());

            m_EnvironmentMock.Setup(x => x.WorkingDirectory).Returns(new DirectoryPath(temporaryDirectory));

            var sut = new ToolManifestReader(m_EnvironmentMock.Object);

            // ACT 
            _ = sut.Load(fileMock.Object);

            // ASSERT
            m_EnvironmentMock.Verify(x => x.WorkingDirectory, Times.Once);
        }

        [Fact]
        public void Load_throws_InvalidToolManifestException_if_file_isnt_valid_JSON()
        {
            // ARRANGE
            using var temporaryDirectory = new TemporaryDirectory();
            var manifestFile = CreateToolManifest(temporaryDirectory, "not json");

            var sut = new ToolManifestReader(m_EnvironmentMock.Object);

            // ACT 
            var ex = Record.Exception(() => sut.Load(manifestFile));

            // ASSERT
            Assert.IsType<InvalidToolManifestException>(ex);
            Assert.IsAssignableFrom<JsonException>(ex.InnerException);
        }

        [Fact]
        public void Load_throws_InvalidToolManifestException_if_file_does_not_have_a_version_property()
        {
            // ARRANGE
            using var temporaryDirectory = new TemporaryDirectory();
            var manifestFile = CreateToolManifest(temporaryDirectory, "{ }");

            var sut = new ToolManifestReader(m_EnvironmentMock.Object);

            // ACT 
            var ex = Record.Exception(() => sut.Load(manifestFile));

            // ASSERT
            Assert.IsType<InvalidToolManifestException>(ex);
        }

        [Fact]
        public void Load_throws_InvalidToolManifestException_if_file_has_an_incompatible_version()
        {
            // ARRANGE
            using var temporaryDirectory = new TemporaryDirectory();
            var manifestFile = CreateToolManifest(temporaryDirectory, @"{ ""version"" : 2 }");

            var sut = new ToolManifestReader(m_EnvironmentMock.Object);

            // ACT 
            var ex = Record.Exception(() => sut.Load(manifestFile));

            // ASSERT
            Assert.IsType<InvalidToolManifestException>(ex);
        }

        [Fact]
        public void Load_returns_empty_manifest_for_emtpy_json_file()
        {
            // ARRANGE
            using var temporaryDirectory = new TemporaryDirectory();
            var manifestFile = CreateToolManifest(temporaryDirectory, @"{ ""version"" : 1 }");

            var sut = new ToolManifestReader(m_EnvironmentMock.Object);

            // ACT 
            var manifest = sut.Load(manifestFile);

            // ASSERT
            Assert.NotNull(manifest);
            Assert.Empty(manifest.Entries);
        }

        [Fact]
        public void Load_returns_expected_tools()
        {
            // ARRANGE
            using var temporaryDirectory = new TemporaryDirectory();
            var manifestFile = CreateToolManifest(
                temporaryDirectory,
                @"{
                  ""version"": 1,
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
                }
"
            );

            var sut = new ToolManifestReader(m_EnvironmentMock.Object);

            // ACT 
            var manifest = sut.Load(manifestFile);

            // ASSERT
            Assert.NotNull(manifest);
            Assert.Collection(
                manifest.Entries.OrderBy(x => x.PackageId),
                entry => Assert.Equal(new ToolManifestEntry("dotnet-format", "5.1.225507"), entry),
                entry => Assert.Equal(new ToolManifestEntry("dotnet-reportgenerator-globaltool", "4.8.12"), entry),
                entry => Assert.Equal(new ToolManifestEntry("nbgv", "3.4.231"), entry)
            );
        }


        private IFile CreateToolManifest(TemporaryDirectory directory, string content)
        {
            var manifestPath = Path.Combine(directory, "dotnet-tools.json");
            File.WriteAllText(manifestPath, content);

            var fileMock = GetFileMock(exists: true, path: manifestPath);
            return fileMock.Object;

        }

        private Mock<IFile> GetFileMock(bool exists, string? path)
        {
            var fileMock = new Mock<IFile>(MockBehavior.Strict);
            fileMock.Setup(x => x.Exists).Returns(exists);
            fileMock.Setup(x => x.Path).Returns(path);
            return fileMock;
        }
    }
}
