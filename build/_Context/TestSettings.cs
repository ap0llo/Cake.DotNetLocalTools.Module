using System;
using Cake.Common;
using Cake.Core.Diagnostics;

namespace Build
{
    public class TestSettings
    {
        private readonly BuildContext m_Context;


        public virtual bool CollectCodeCoverage => m_Context.Argument("collect-code-coverage", true);


        public TestSettings(BuildContext context)
        {
            m_Context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public void PrintToLog(int indentWidth = 0)
        {
            string prefix = new String(' ', indentWidth);
            m_Context.Log.Information($"{prefix}{nameof(CollectCodeCoverage)}: {CollectCodeCoverage}");
        }
    }
}
