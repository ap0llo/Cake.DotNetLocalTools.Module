using System;
using Cake.Frosting;

namespace Cake.LocalTools.Module
{
    public static class CakeHostExtensions
    {
        public static CakeHost InstallToolsFromManifest(this CakeHost host, string manifestPath)
        {
            var toolUri = $"{ToolManifestPackageInstaller.ToolManifestScheme}:?package={manifestPath}";
            return host.InstallTool(new Uri(toolUri));
        }
    }
}
