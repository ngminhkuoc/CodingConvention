﻿using System.Diagnostics;
using System.Text.RegularExpressions;
using CodingConvention.Common;
using EnvDTE;

namespace CodingConvention.Helpers
{
    /// <summary>
    /// A set of helper methods focused around code comments.
    /// </summary>
    internal static class CodeCommentHelper
    {
        public const int CopyrightExtraIndent = 4;
        public const char KeepTogetherSpacer = '\a';
        public const char Spacer = ' ';

        internal static string FakeToSpace(string value) => value.Replace(KeepTogetherSpacer, Spacer);

        /// <summary>
        /// Get the comment prefix (regex) for the given document's language.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns>The comment prefix regex, without trailing spaces.</returns>
        internal static string GetCommentPrefix(TextDocument document) => GetCommentPrefixForLanguage(document.GetCodeLanguage());

        /// <summary>
        /// Get the comment prefix (regex) for the specified code language.
        /// </summary>
        /// <param name="codeLanguage">The code language.</param>
        /// <returns>The comment prefix regex, without trailing spaces.</returns>
        internal static string GetCommentPrefixForLanguage(CodeLanguage codeLanguage)
        {
            switch (codeLanguage)
            {
                case CodeLanguage.CPlusPlus:
                case CodeLanguage.CSharp:
                case CodeLanguage.CSS:
                case CodeLanguage.FSharp:
                case CodeLanguage.JavaScript:
                case CodeLanguage.LESS:
                case CodeLanguage.PHP:
                case CodeLanguage.SCSS:
                case CodeLanguage.TypeScript:
                    return "///?";

                case CodeLanguage.PowerShell:
                case CodeLanguage.R:
                    return "#+";

                case CodeLanguage.VisualBasic:
                    return "'+";

                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the regex for matching a complete comment line.
        /// </summary>
        internal static Regex GetCommentRegex(CodeLanguage codeLanguage, bool includePrefix = true)
        {
            string prefix = null;

            if (includePrefix)
            {
                prefix = GetCommentPrefixForLanguage(codeLanguage);
                if (prefix == null)
                {
                    Debug.Fail("Attempting to create a comment regex for a document that has no comment prefix specified.");
                }

                // Be aware of the added space to the prefix. When prefix is added, we should take
                // care not to match code comment lines.
                prefix = string.Format(@"(?<prefix>[\t ]*{0})(?<initialspacer>( |\t|\r|\n|$))?", prefix);
            }

            var pattern = string.Format(@"^{0}(?<indent>[\t ]*)(?<line>(?<listprefix>[-=\*\+]+[ \t]*|\w+[\):][ \t]+|\d+\.[ \t]+)?((?<words>[^\t\r\n ]+)*[\t ]*)*)\r*\n?$", prefix);
            return new Regex(pattern, RegexOptions.ExplicitCapture | RegexOptions.Multiline);
        }

        internal static bool IsCommentLine(EditPoint point) => LineMatchesRegex(point, GetCommentRegex(point.GetCodeLanguage())).Success;

        internal static Match LineMatchesRegex(EditPoint point, Regex regex)
        {
            var line = point.GetLine();
            var match = regex.Match(line);
            return match;
        }

        internal static string SpaceToFake(string value) => value.Replace(Spacer, KeepTogetherSpacer);
    }
}
