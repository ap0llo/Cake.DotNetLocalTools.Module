using System;
using System.Collections.Generic;
using System.IO;
using Cake.Core;
using Cake.Core.IO;
using Newtonsoft.Json;

namespace Cake.LocalTools
{
    /// <summary>
    /// Default implementation of <see cref="IToolManifestReader"/>
    /// </summary>
    internal class ToolManifestReader : IToolManifestReader
    {
        private class ToolManifestDto
        {
            [JsonProperty("version")]
            public int? Version { get; set; } = null;

            [JsonProperty("tools")]
            public Dictionary<string, ToolManifestEntryDto> Tools { get; set; } = new();
        }

        private class ToolManifestEntryDto
        {
            [JsonProperty("version")]
            public string Version { get; set; } = "";
        }

        private readonly ICakeEnvironment m_Environment;


        public ToolManifestReader(ICakeEnvironment environment)
        {
            m_Environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        /// <inheritdoc />
        public ToolManifest Load(IFile file)
        {
            if (file is null)
                throw new ArgumentNullException(nameof(file));

            if (!file.Exists)
                throw new FileNotFoundException($"Tool manifest file '{file.Path}' does not exist");

            var path = file.Path.MakeAbsolute(m_Environment).FullPath;

            var json = File.ReadAllText(path);

            ToolManifestDto? dto;
            try
            {
                dto = JsonConvert.DeserializeObject<ToolManifestDto>(json);
            }
            catch (JsonException ex)
            {
                throw new InvalidToolManifestException($"Failed to deserialize tool manifest at '{path}'", ex);
            }

            var manifestVersion = dto!.Version;
            if (manifestVersion != 1)
            {
                throw new InvalidToolManifestException($"Tool manifest at '{path}' has unexpected version '{manifestVersion}' (expected '1').");
            }

            var manifest = new ToolManifest();

            foreach (var kvp in dto.Tools)
            {
                manifest.Add(kvp.Key, kvp.Value.Version);
            }

            return manifest;
        }
    }
}
