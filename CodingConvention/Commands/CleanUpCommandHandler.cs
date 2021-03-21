using System.ComponentModel.Composition;
using CodingConvention.Services;
using EnvDTE;
using EnvDTE80;
using Microsoft;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace CodingConvention.Commands
{
    [Export(typeof(ICommandHandler))]
    [Name(nameof(CleanUpCommandHandler))]
    [ContentType("code")]
    [TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
    [Order(After = "CodeCleanUpProfileCommandHandler")]
    internal class CleanUpCommandHandler : ICommandHandler<CleanUpCommandArgs>
    {
        [Import]
        internal SVsServiceProvider ServiceProvider = null;

        public bool ExecuteCommand(CleanUpCommandArgs args, CommandExecutionContext executionContext)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var ide = (DTE2)ServiceProvider.GetService(typeof(DTE));

            Assumes.Present(ide); // Throw an Exception in case ide is null

            var commandManager = CleanUpManager.GetInstance(ide);
            commandManager.Execute(ide.ActiveDocument);

            return true;
        }

        public CommandState GetCommandState(CleanUpCommandArgs args) => CommandState.Available;

        public string DisplayName => nameof(CleanUpCommandHandler);
    }
}
