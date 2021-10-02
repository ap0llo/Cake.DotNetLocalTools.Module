using System;
using Cake.Core.Packaging;

namespace Cake.DotNetLocalTools.Module
{
    internal sealed class ToolManifestEntry : IEquatable<ToolManifestEntry>
    {
        /// <summary>
        /// Gets the id of the tool package
        /// </summary>
        public string PackageId { get; }

        /// <summary>
        /// Gets the version of the tool package
        /// </summary>
        public string Version { get; }


        public ToolManifestEntry(string packageId, string version)
        {
            if (String.IsNullOrWhiteSpace(packageId))
                throw new ArgumentException("Value must not be null or whitespace", nameof(packageId));

            if (String.IsNullOrWhiteSpace(version))
                throw new ArgumentException("Value must not be null or whitespace", nameof(version));

            PackageId = packageId;
            Version = version;
        }

        /// <summary>
        /// Converts the tool manifest entry to a <see cref="PackageReference"/>.
        /// </summary>
        /// <returns></returns>
        public PackageReference ToPackageReference() =>
            new PackageReference($"dotnet:?package={PackageId}&version={Version}");


        /// <inheritdoc />
        public override string ToString() => $"{PackageId}, version {Version}";

        /// <inheritdoc />
        public override bool Equals(object obj) => Equals(obj as ToolManifestEntry);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = StringComparer.OrdinalIgnoreCase.GetHashCode(PackageId) * 397;
                hash ^= StringComparer.OrdinalIgnoreCase.GetHashCode(Version);
                return hash;
            }
        }

        /// <inheritdoc />
        public bool Equals(ToolManifestEntry? other)
        {
            return other is not null &&
                StringComparer.OrdinalIgnoreCase.Equals(PackageId, other.PackageId) &&
                StringComparer.OrdinalIgnoreCase.Equals(Version, other.Version);
        }
    }
}
