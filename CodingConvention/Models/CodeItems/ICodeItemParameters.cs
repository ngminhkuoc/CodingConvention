using EnvDTE;
using System.Collections.Generic;

namespace CodingConvention.Models.CodeItems
{
    /// <summary>
    /// An interface for code items that support having parameters.
    /// </summary>
    internal interface ICodeItemParameters : ICodeItem
    {
        /// <summary>
        /// Gets the parameters.
        /// </summary>
        IEnumerable<CodeParameter> Parameters { get; }
    }
}