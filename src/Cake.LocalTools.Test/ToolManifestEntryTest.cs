using System;
using Xunit;

namespace Cake.LocalTools.Test
{
    /// <summary>
    /// Tests for <see cref="ToolManifestEntry"/>
    /// </summary>
    public class ToolManifestEntryTest
    {
        [Theory]
        [InlineData("SomePackage", "1.2.3", "SomePackage", "1.2.3", true)]
        [InlineData("SomePackage", "1.2.3", "somepackage", "1.2.3", true)]
        [InlineData("SomePackage", "1.2.3-alpha", "SomePackage", "1.2.3-ALPHA", true)]
        [InlineData("SomePackage", "1.2.3", "SomeOtherPackage", "1.2.3", false)]
        [InlineData("SomePackage", "1.2.3", "SomePackage", "4.5.6", false)]
        public void Equality_methods_return_expected_results(string packageId1, string version1, string packageId2, string version2, bool expectedEqual)
        {
            // ARRANGE
            var entry1 = new ToolManifestEntry(packageId1, version1);
            var entry2 = new ToolManifestEntry(packageId2, version2);

            // ACT 
            var actualEqual = entry1.Equals(entry2);
            var hashCode1 = entry1.GetHashCode();
            var hashCode2 = entry2.GetHashCode();

            // ASSERT
            Assert.Equal(expectedEqual, actualEqual);
            Assert.Equal(entry1.Equals(entry2), entry2.Equals(entry1));
            if (expectedEqual)
            {
                Assert.Equal(hashCode1, hashCode2);
            }
        }

        [Theory]
        [InlineData("SomePackage", "1.2.3", "dotnet:?package=SomePackage&version=1.2.3")]
        public void ToPackageReference_returns_expected_package_reference(string packageId, string version, string expected)
        {
            // ARRANGE
            var sut = new ToolManifestEntry(packageId, version);

            // ACT
            var packageReference = sut.ToPackageReference();

            // ASSERT
            Assert.Equal("dotnet", packageReference.Scheme);
            Assert.Equal(expected, packageReference.OriginalString);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\t")]
        public void PackageId_must_not_be_null_or_whitespace(string packageId)
        {
            // ARRANGE

            // ACT 
            var ex = Record.Exception(() => new ToolManifestEntry(packageId, "1.2.3"));

            // ASSERT
            var argumentException = Assert.IsType<ArgumentException>(ex);
            Assert.Equal("packageId", argumentException.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\t")]
        public void Version_must_not_be_null_or_whitespace(string version)
        {
            // ARRANGE

            // ACT 
            var ex = Record.Exception(() => new ToolManifestEntry("SomePackage", version));

            // ASSERT
            var argumentException = Assert.IsType<ArgumentException>(ex);
            Assert.Equal("version", argumentException.ParamName);
        }
    }
}
