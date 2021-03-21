﻿using System.Collections.Generic;
using System.Linq;
using CodingConvention.Helpers;
using CodingConvention.Models.CodeItems;
using EnvDTE;
using EnvDTE80;

namespace CodingConvention.Services
{
    internal class CodeModelBuilder
    {
        private readonly CodeModelService _codeModelService;
        private readonly DTE2 _ide;
        private static CodeModelBuilder _instance;

        private CodeModelBuilder(DTE2 ide)
        {
            _ide = ide;
            _codeModelService = CodeModelService.GetInstance();
        }

        /// <summary>
        /// Gets an instance of the <see cref="CodeModelBuilder" /> class.
        /// </summary>
        /// <param name="ide">The hosting package.</param>
        /// <returns>An instance of the <see cref="CodeModelBuilder" /> class.</returns>
        internal static CodeModelBuilder GetInstance(DTE2 ide) => _instance ?? (_instance = new CodeModelBuilder(ide));

        /// <summary>
        /// Walks the given document and constructs a <see cref="IList<BaseCodeItem>" /> of CodeItems
        /// within it including regions.
        /// </summary>
        /// <param name="document">The document to walk.</param>
        /// <returns>The set of code items within the document, including regions.</returns>
        internal IList<BaseCodeItem> RetrieveAllCodeItems(Document document)
        {
            var codeItems = new List<BaseCodeItem>();

            var fileCodeModel = RetrieveFileCodeModel(document.ProjectItem);
            RetrieveCodeItems(codeItems, fileCodeModel);

            var codeRegions = _codeModelService.RetrieveCodeRegions(document.GetTextDocument());
            codeItems.AddRange(codeRegions);

            return codeItems;
        }

        /// <summary>
        /// Walks the given FileCodeModel, turning CodeElements into code items within the specified
        /// code items set.
        /// </summary>
        /// <param name="codeItems">The code items set for accumulation.</param>
        /// <param name="fileCodeModel">The FileCodeModel to walk.</param>
        private static void RetrieveCodeItems(IList<BaseCodeItem> codeItems, FileCodeModel fileCodeModel)
        {
            if (fileCodeModel != null && fileCodeModel.CodeElements != null)
            {
                RetrieveCodeItemsFromElements(codeItems, fileCodeModel.CodeElements);
            }
        }

        /// <summary>
        /// Retrieves code items from each specified code element into the specified code items set.
        /// </summary>
        /// <param name="codeItems">The code items set for accumulation.</param>
        /// <param name="codeElements">The CodeElements to walk.</param>
        private static void RetrieveCodeItemsFromElements(IList<BaseCodeItem> codeItems, CodeElements codeElements)
        {
            foreach (CodeElement child in codeElements)
            {
                RetrieveCodeItemsRecursively(codeItems, child);
            }
        }

        /// <summary>
        /// Recursive method for creating a code item for the specified code element, adding it to
        /// the specified code items set and recursing into all of the code element's children.
        /// </summary>
        /// <param name="codeItems">The code items set for accumulation.</param>
        /// <param name="codeElement">The CodeElement to walk (add and recurse).</param>
        private static void RetrieveCodeItemsRecursively(IList<BaseCodeItem> codeItems, CodeElement codeElement)
        {
            var codeItem = FactoryCodeItems.CreateCodeItemElement(codeElement);

            if (codeItem != null)
            {
                codeItems.Add(codeItem);
            }

            if (codeElement.Children != null)
            {
                RetrieveCodeItemsFromElements(codeItems, codeElement.Children);
            }
        }

        /// <summary>
        /// Attempts to return the FileCodeModel associated with the specified project item.
        /// </summary>
        /// <param name="projectItem">The project item.</param>
        /// <returns>The associated FileCodeModel, otherwise null.</returns>
        private FileCodeModel RetrieveFileCodeModel(ProjectItem projectItem)
        {
            if (projectItem == null)
            {
                return null;
            }

            if (projectItem.FileCodeModel != null)
            {
                return projectItem.FileCodeModel;
            }

            // If this project item is part of a shared project, retrieve the FileCodeModel via a similar platform project item.
            const string sharedProjectTypeGUID = "{d954291e-2a0b-460d-934e-dc6b0785db48}";
            var containingProject = projectItem.ContainingProject;

            if (containingProject != null && containingProject.Kind != null &&
                containingProject.Kind.ToLowerInvariant() == sharedProjectTypeGUID)
            {
                var similarProjectItems = SolutionHelper.GetSimilarProjectItems(_ide, projectItem);
                var fileCodeModel = similarProjectItems.FirstOrDefault(x => x.FileCodeModel is object)?.FileCodeModel;

                return fileCodeModel;
            }

            return null;
        }
    }
}
