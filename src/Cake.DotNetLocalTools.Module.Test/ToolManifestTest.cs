using System.Linq;
using Xunit;

namespace Cake.DotNetLocalTools.Module.Test
{
    /// <summary>
    /// Tests for <see cref="ToolManifest"/>
    /// </summary>
    public class ToolManifestTest
    {
        [Fact]
        public void Entries_is_initiall_emtpy()
        {
            // ARRANGE
            var sut = new ToolManifest();

            // ACT 

            // ASSERT
            Assert.NotNull(sut.Entries);
            Assert.Empty(sut.Entries);
        }

        [Fact]
        public void Add_adds_the_expected_entry()
        {
            // ARRANGE
            var sut = new ToolManifest();

            // ACT 
            sut.Add("SomePackage", "1.2.3");
            sut.Add("SomeOtherPackage", "4.5.6");

            // ASSERT
            Assert.Collection(
                sut.Entries.OrderBy(x => x.PackageId),
                entry => Assert.Equal(new ToolManifestEntry("SomeOtherPackage", "4.5.6"), entry),
                entry => Assert.Equal(new ToolManifestEntry("SomePackage", "1.2.3"), entry)
            );
        }

        [Fact]
        public void A_entry_can_only_be_added_once()
        {
            // ARRANGE
            var sut = new ToolManifest();

            // ACT 
            sut.Add("SomePackage", "1.2.3");
            sut.Add("SomePackage", "1.2.3");

            // ASSERT
            var entry = Assert.Single(sut.Entries);
            Assert.Equal("SomePackage", entry.PackageId);
            Assert.Equal("1.2.3", entry.Version);
        }
    }
}
