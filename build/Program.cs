using System;
using Build;
using Cake.AzurePipelines.Module;
using Cake.Frosting;

return new CakeHost()
            .UseContext<BuildContext>()
            .UseModule<AzurePipelinesModule>()
            // Since this build is building the local tools module, the build cannot use it.
            // Instead install all the required tools individually
            .InstallTool(new Uri("dotnet:?package=Grynwald.ChangeLog&version=0.4.135"))  
            .Run(args);
