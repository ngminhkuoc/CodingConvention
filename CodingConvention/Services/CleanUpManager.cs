using System.Linq;
using CodingConvention.Helpers;
using CodingConvention.Models.CodeItems;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace CodingConvention.Services
{
    internal sealed class CleanUpManager
    {
        private readonly BlankLineInsertService _blankLineInsertService;
        private readonly CodeItemReorganizer _codeItemReorganizer;
        private readonly CodeItemRetriever _codeItemRetriever;
        private readonly CodeRegionService _codeRegionService;
        private readonly CodeTreeBuilder _codeTreeBuilder;
        private readonly DTE2 _ide;
        private static CleanUpManager _instance;

        private CleanUpManager(DTE2 ide)
        {
            _ide = ide;

            _codeItemRetriever = CodeItemRetriever.GetInstance(ide);
            _codeItemReorganizer = CodeItemReorganizer.GetInstance();
            _codeTreeBuilder = CodeTreeBuilder.GetInstance();
            _codeRegionService = CodeRegionService.GetInstance();
            _blankLineInsertService = BlankLineInsertService.GetInstance();
        }

        /// <summary>
        /// Execute the  Cleanup logic
        /// </summary>
        /// <param name="document">The active document which is being worked on</param>
        internal void Execute(Document document)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            new UndoTransactionHelper(_ide, document.Name).Run(() =>
            {
                var codeItems = _codeItemRetriever.Retrieve(document).Where(item => !(item is CodeItemUsingStatement));
                codeItems = _codeRegionService.CleanupExistingRegions(codeItems);
                codeItems = _codeTreeBuilder.Build(codeItems);
                _codeItemReorganizer.Reorganize(codeItems);
                _blankLineInsertService.InsertPaddings(codeItems);

#if DEBUG
                OutputWindowHelper.PrintCodeItems(codeItems);
#endif
            });
        }

        internal static CleanUpManager GetInstance(DTE2 ide) => _instance ?? (_instance = new CleanUpManager(ide));
    }
}
