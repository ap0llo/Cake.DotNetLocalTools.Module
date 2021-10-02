using System;
using System.IO;
using System.Linq;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Core.Packaging;
using Moq;
using Xunit;

namespace Cake.DotNetLocalTools.Module.Test
{
    /// <summary>
    /// Tests for <see cref="ToolManifestPackageInstaller"/>
    /// </summary>
    public class ToolManifestPackageInstallerTest
    {
        private readonly Mock<ICakeLog> m_LogMock;
        private readonly Mock<IDotNetToolPackageInstaller> m_PackageInstallerMock;
        private readonly Mock<IFileSystem> m_FileSystemMock;
        private readonly Mock<IToolManifestReader> m_ToolManifestReaderMock;


        public ToolManifestPackageInstallerTest()
        {
            m_LogMock = new Mock<ICakeLog>(MockBehavior.Strict);
            m_LogMock.Setup(x => x.Write(It.IsAny<Verbosity>(), It.IsAny<LogLevel>(), It.IsAny<string>(), It.IsAny<object[]>()));

            m_FileSystemMock = new Mock<IFileSystem>(MockBehavior.Strict);

            m_ToolManifestReaderMock = new Mock<IToolManifestReader>(MockBehavior.Strict);

            m_PackageInstallerMock = new Mock<IDotNetToolPackageInstaller>(MockBehavior.Strict);
        }


        [Theory]
        [InlineData(PackageType.Unspecified)]
        [InlineData(PackageType.Addin)]
        [InlineData(PackageType.Module)]
        public void CanInstall_returns_false_if_type_is_not_tool(PackageType type)
        {
            // ARRANGE
            var sut = CreateInstance();

            // ACT 
            var canInstall = sut.CanInstall(new PackageReference("toolmanifest:?package=dotnet-tools.json"), type);

            // ASSERT
            Assert.False(canInstall);
        }

        [Theory]
        [InlineData("dotnet:?package=packageid")]
        [InlineData("nuget:?package=packageid")]
        [InlineData("some-other-scheme:?package=packageid")]
        public void CanInstall_returns_false_for_unsupported_package_references(string reference)
        {
            // ARRANGE
            var sut = CreateInstance();

            // ACT 
            var canInstall = sut.CanInstall(new PackageReference(reference), PackageType.Tool);

            // ASSERT
            Assert.False(canInstall);
        }

        [Theory]
        [InlineData("toolmanifest:?package=dotnet-tools.json")]
        public void CanInstall_returns_true_for_supported_package_references(string reference)
        {
            // ARRANGE
            var sut = CreateInstance();

            // ACT 
            var canInstall = sut.CanInstall(new PackageReference(reference), PackageType.Tool);

            // ASSERT
            Assert.True(canInstall);
        }


        [Theory]
        [InlineData("toolmanifest", PackageType.Unspecified)]
        [InlineData("toolmanifest", PackageType.Addin)]
        [InlineData("toolmanifest", PackageType.Module)]
        [InlineData("dotnet", PackageType.Tool)]
        [InlineData("nuget", PackageType.Tool)]
        [InlineData("someOtherScheme", PackageType.Tool)]
        public void Install_throws_InvalidOperationException_for_unsupported_parameter_values(string scheme, PackageType type)
        {
            // ARRANGE
            var sut = CreateInstance();

            var package = new PackageReference($"{scheme}:?package=dotnet-tools.json");
            var path = new DirectoryPath("tools");

            // ACT 
            var ex = Record.Exception(() => sut.Install(package, type, path));

            // ASSERT
            Assert.IsType<InvalidOperationException>(ex);
        }

        [Theory]
        [InlineData("toolmanifest:?package=")]
        public void Install_throws_ArgumentException_if_package_reference_cannot_be_parsed(string packageReference)
        {
            // ARRANGE
            var sut = CreateInstance();

            var package = new PackageReference(packageReference);
            var path = new DirectoryPath("tools");

            // ACT
            var ex = Record.Exception(() => sut.Install(package, PackageType.Tool, path));

            // ASSERT
            Assert.IsType<ArgumentException>(ex);
        }

        [Fact]
        public void Install_throws_FileNotFoundException_if_tool_manifest_does_not_exist()
        {
            // ARRANGE
            var fileMock = new Mock<IFile>(MockBehavior.Strict);
            fileMock.Setup(x => x.Exists).Returns(false);
            fileMock.Setup(x => x.Path).Returns(new FilePath("dotnet-tools.json"));

            m_FileSystemMock
                .Setup(x => x.GetFile(It.IsAny<FilePath>()))
                .Returns(fileMock.Object);

            var sut = CreateInstance();

            var package = new PackageReference("toolmanifest:?package=dotnet-tools.json");
            var path = new DirectoryPath("tools");

            // ACT
            var ex = Record.Exception(() => sut.Install(package, PackageType.Tool, path));

            // ASSERT
            Assert.IsType<FileNotFoundException>(ex);
        }

        [Fact]
        public void Install_installs_all_packages_listed_in_the_tool_manifest()
        {
            // ARRANGE
            var manifestPath = new FilePath("path/to/dotnet-tools.json");

            var manifest = new ToolManifest();
            manifest.Add("package1", "1.2.3");
            manifest.Add("package2", "4.5.6");

            var fileMock = new Mock<IFile>(MockBehavior.Strict);
            fileMock.Setup(x => x.Exists).Returns(true);
            fileMock.Setup(x => x.Path).Returns(manifestPath);

            m_FileSystemMock
                .Setup(x => x.GetFile(It.Is<FilePath>(path => path.ToString() == manifestPath.ToString())))
                .Returns(fileMock.Object);

            m_ToolManifestReaderMock
                .Setup(x => x.Load(It.Is<IFile>(file => file.Path.ToString() == manifestPath.ToString())))
                .Returns(manifest);

            m_PackageInstallerMock
                .Setup(x =>
                    x.Install(
                        It.Is<PackageReference>(pkg => pkg.OriginalString == $"dotnet:?package=package1&version=1.2.3"),
                        PackageType.Tool,
                        It.IsAny<DirectoryPath>()
                ))
                .Returns(new[]
                {
                    GetFileMock(exists: true, path: "file1").Object,
                    GetFileMock(exists: true, path: "file2").Object
                });

            m_PackageInstallerMock
                .Setup(x =>
                    x.Install(
                        It.Is<PackageReference>(pkg => pkg.OriginalString == $"dotnet:?package=package2&version=4.5.6"),
                        PackageType.Tool,
                        It.IsAny<DirectoryPath>()
                ))
                .Returns(new[]
                {
                    GetFileMock(exists: true, path: "file3").Object,
                    GetFileMock(exists: true, path: "file4").Object
                });

            var sut = CreateInstance();

            var package = new PackageReference($"toolmanifest:?package={manifestPath}");
            var path = new DirectoryPath("tools");

            // ACT 
            var files = sut.Install(package, PackageType.Tool, path);

            // ASSERT
            Assert.NotNull(files);
            Assert.Collection(
                files.Select(x => x.Path.GetFilename().ToString()).OrderBy(x => x),
                name => Assert.Equal("file1", name),
                name => Assert.Equal("file2", name),
                name => Assert.Equal("file3", name),
                name => Assert.Equal("file4", name)
            );
            m_PackageInstallerMock.Verify(x => x.Install(It.IsAny<PackageReference>(), It.IsAny<PackageType>(), It.IsAny<DirectoryPath>()), Times.Exactly(manifest.Entries.Count));
        }


        private ToolManifestPackageInstaller CreateInstance() =>
            new(m_LogMock.Object, m_FileSystemMock.Object, m_ToolManifestReaderMock.Object, m_PackageInstallerMock.Object);

        private Mock<IFile> GetFileMock(bool exists, string? path)
        {
            var fileMock = new Mock<IFile>(MockBehavior.Strict);
            fileMock.Setup(x => x.Exists).Returns(exists);
            fileMock.Setup(x => x.Path).Returns(path);
            return fileMock;
        }
    }
}
