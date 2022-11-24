using System;
using System.Collections.Generic;
using Cake.Core.Diagnostics;
using Cake.Core.IO;

namespace Build
{
    public class CodeFormattingSettings
    {
        private readonly BuildContext m_Context;


        public virtual bool EnableAutomaticFormatting => true;

        public ICollection<DirectoryPath> ExcludedDirectories { get; set; } = new List<DirectoryPath>();


        public CodeFormattingSettings(BuildContext context)
        {
            m_Context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public void PrintToLog(int indentWidth)
        {
            string prefix = new String(' ', indentWidth);

            m_Context.Log.Information($"{prefix}{nameof(EnableAutomaticFormatting)}: {EnableAutomaticFormatting}");

            string innerPrefix = new String(' ', indentWidth + 2);
            m_Context.Log.Information($"{prefix}{nameof(ExcludedDirectories)}:");
            var index = 0;
            foreach (var path in ExcludedDirectories ?? Array.Empty<DirectoryPath>())
            {
                m_Context.Log.Information($"{innerPrefix}{nameof(ExcludedDirectories)}[{index}]: {path}");
                index++;
            }
        }
    }
}
