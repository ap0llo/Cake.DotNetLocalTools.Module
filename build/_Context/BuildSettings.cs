using System;
using Cake.Common;
using Cake.Common.Tools.DotNetCore.MSBuild;
using Cake.Core.Diagnostics;

namespace Build
{
    public class BuildSettings
    {
        private readonly BuildContext m_Context;


        /// <summary>
        /// Gets the configuration to build (Debug/Relesae)
        /// </summary>
        public string Configuration => m_Context.Argument("configuration", "Release");

        /// <summary>
        /// Determines whether to use deterministic build settings
        /// </summary>
        /// <remarks>
        /// Determinisitc build can be enabled/disabled using the --deterministic command line switch.
        /// Wehn running in the CI environment, it is enabled by default.
        /// In other environments it is disabled by default.
        /// </remarks>
        public bool Deterministic => m_Context.Argument("deterministic", m_Context.IsRunningInCI);



        public BuildSettings(BuildContext context)
        {
            m_Context = context ?? throw new ArgumentNullException(nameof(context));
        }



        public DotNetCoreMSBuildSettings GetDefaultMSBuildSettings() => new()
        {
            TreatAllWarningsAs = MSBuildTreatAllWarningsAs.Error
        };


        public void PrintToLog(int indentWidth = 0)
        {
            string prefix = new String(' ', indentWidth);

            m_Context.Log.Information($"{prefix}{nameof(Configuration)}: {Configuration}");
            m_Context.Log.Information($"{prefix}{nameof(Deterministic)}: {Deterministic}");
        }
    }
}
