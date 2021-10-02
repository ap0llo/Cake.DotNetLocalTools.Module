using System.Diagnostics;
using Cake.Core.Annotations;
using Cake.Core.Composition;
using Cake.Core.Packaging;
using Cake.LocalTools;

[assembly: CakeModule(typeof(LocalToolsModule))]

namespace Cake.LocalTools
{
    public class LocalToolsModule : ICakeModule
    {
        public void Register(ICakeContainerRegistrar registrar)
        {
            registrar.RegisterType<ToolManifestPackageInstaller>().As<IPackageInstaller>().Singleton();
            registrar.RegisterType<ToolManifestReader>().As<IToolManifestReader>().Singleton();
            registrar.RegisterType<DotNetToolPackageInstallerWrapper>().As<IDotNetToolPackageInstaller>().Singleton();
        }
    }
}
