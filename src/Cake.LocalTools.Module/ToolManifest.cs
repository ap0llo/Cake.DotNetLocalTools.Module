using System;
using System.Collections.Generic;

namespace Cake.LocalTools.Module
{
    internal class ToolManifest
    {
        private readonly HashSet<ToolManifestEntry> m_Entries = new();


        /// <summary>
        /// Gets the tool manifest's entries
        /// </summary>
        public IReadOnlyCollection<ToolManifestEntry> Entries => m_Entries;


        /// <summary>
        /// Adds the specified tool package to the tool manifest
        /// </summary>
        /// <param name="packageId">The tool's package id.</param>
        /// <param name="version">The tool's version.</param>
        public void Add(string packageId, string version)
        {
            if (String.IsNullOrWhiteSpace(packageId))
                throw new ArgumentException("Value must not be null or whitespace", nameof(packageId));

            if (String.IsNullOrWhiteSpace(version))
                throw new ArgumentException("Value must not be null or whitespace", nameof(version));

            m_Entries.Add(new(packageId, version));
        }
    }
}
