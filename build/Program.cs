using Build;
using Cake.AzurePipelines.Module;
using Cake.Frosting;

return new CakeHost()
            .UseContext<BuildContext>()
            .UseModule<AzurePipelinesModule>()
            .Run(args);
