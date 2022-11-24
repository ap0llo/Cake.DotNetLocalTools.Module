using System;
using Build;
using Cake.AzurePipelines.Module;
using Cake.Frosting;

return new CakeHost()
            .UseContext<BuildContext>()
            // Usage of AzurePipelinesModule temporarily commented out since it does not yet support Cake 3.0
            //.UseModule<AzurePipelinesModule>()
            // Since this build is building the local tools module, the build cannot use it.
            // Instead install all the required tools individually
            .InstallTool(new Uri("dotnet:?package=Grynwald.ChangeLog&version=1.1.118"))
            .InstallTool(new Uri("dotnet:?package=dotnet-reportgenerator-globaltool&version=5.1.12"))
            .Run(args);
