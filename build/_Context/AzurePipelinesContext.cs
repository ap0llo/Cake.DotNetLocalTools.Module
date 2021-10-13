using System;
using Cake.Common.Build;
using Cake.Core.Diagnostics;

namespace Build
{
    public class AzurePipelinesContext
    {
        public class ArtifactNameSettings
        {
            private readonly BuildContext m_Context;


            /// <summary>
            /// The name of the main artifact
            /// </summary>
            public string Binaries => "Binaries";

            /// <summary>
            /// The artifact name under which to save test result files
            /// </summary>
            public string TestResults => "TestResults";

            /// <summary>
            /// The artifact name for the auto-generated change log.
            /// </summary>
            public string ChangeLog => "ChangeLog";


            public ArtifactNameSettings(BuildContext context)
            {
                m_Context = context ?? throw new ArgumentNullException(nameof(context));
            }


            public void PrintToLog(int indentWidth = 0)
            {
                string prefix = new String(' ', indentWidth);

                m_Context.Log.Information($"{prefix}{nameof(Binaries)}: {Binaries}");
                m_Context.Log.Information($"{prefix}{nameof(TestResults)}: {TestResults}");
                m_Context.Log.Information($"{prefix}{nameof(ChangeLog)}: {ChangeLog}");
            }
        }

        private readonly BuildContext m_Context;


        public ArtifactNameSettings ArtifactNames { get; }

        public bool IsActive =>
            m_Context.AzurePipelines().IsRunningOnAzurePipelines ||
            m_Context.AzurePipelines().IsRunningOnAzurePipelinesHosted;


        public AzurePipelinesContext(BuildContext context)
        {
            m_Context = context ?? throw new ArgumentNullException(nameof(context));
            ArtifactNames = new(context);
        }


        public void PrintToLog(int indentWidth = 0)
        {
            string prefix = new String(' ', indentWidth);

            m_Context.Log.Information($"{prefix}{nameof(IsActive)}: {IsActive}");

            m_Context.Log.Information($"{prefix}{nameof(ArtifactNames)}:");
            ArtifactNames.PrintToLog(indentWidth + 2);
        }
    }
}
