using EnvDTE;
using EnvDTE80;
using System;

namespace CodingConvention.Models.CodeItems
{
    /// <summary>
    /// The representation of a code class.
    /// </summary>
    internal class CodeItemClass : BaseCodeItemElementParent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeItemClass" /> class.
        /// </summary>
        public CodeItemClass()
        {
            _Access = LazyTryDefault(() => CodeClass?.Access ?? vsCMAccess.vsCMAccessPublic);

            _Attributes = LazyTryDefault(() => CodeClass?.Attributes);

            _DocComment = LazyTryDefault(() => CodeClass?.DocComment);

            _IsStatic = LazyTryDefault(() => CodeClass != null && CodeClass.IsShared);

            _Namespace = LazyTryDefault(() => CodeClass?.Namespace?.Name);

            _TypeString = new Lazy<string>(() => "class");
        }

        /// <summary>
        /// Gets the kind.
        /// </summary>
        public override KindCodeItem Kind => KindCodeItem.Class;

        /// <summary>
        /// Gets or sets the underlying VSX CodeClass.
        /// </summary>
        public CodeClass2 CodeClass { get; set; }

    }
}