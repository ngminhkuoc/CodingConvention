using Microsoft.VisualStudio.Editor.Commanding;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.Commanding;
using System.ComponentModel.Composition;

namespace CodingConvention.Commands
{
    // Bind a CmdSet to the CodeCleanUpProfileCommandArgs
    [Export(typeof(CommandBindingDefinition))]
    [CommandBinding(PackageGuids.CodingConventionPackageCmdSetString, PackageIds.CleanUpCommandId, typeof(CleanUpCommandArgs))]

    public class CleanUpCommandArgs : EditorCommandArgs
    {
        public CleanUpCommandArgs(ITextView textView, ITextBuffer textBuffer) 
            : base(textView, textBuffer)
        {
        }
    }
}
