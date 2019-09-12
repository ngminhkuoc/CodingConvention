﻿using EnvDTE;
using PinnacleCodingConvention.Common;
using Document = EnvDTE.Document;

namespace PinnacleCodingConvention.Helpers
{
    /// <summary>
    /// A set of extension methods for <see cref="Document" />.
    /// </summary>
    internal static class DocumentExtension
    {
        /// <summary>
        /// Gets the <see cref="CodeLanguage"/> for this document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns>A <see cref="CodeLanguage"/>.</returns>
        internal static CodeLanguage GetCodeLanguage(this Document document) => CodeLanguageHelper.GetCodeLanguage(document.Language);

        /// <summary>
        /// Attempts to get the TextDocument associated with the specified document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns>The associated text document, otherwise null.</returns>
        internal static TextDocument GetTextDocument(this Document document) => document.Object("TextDocument") as TextDocument;

        /// <summary>
        /// Determines if the specified document is external to the solution.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns>True if the document is external, otherwise false.</returns>
        internal static bool IsExternal(this Document document)
        {
            var projectItem = document.ProjectItem;

            return projectItem == null || projectItem.IsExternal();
        }
    }
}
