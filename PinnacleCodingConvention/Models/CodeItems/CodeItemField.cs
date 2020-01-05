using EnvDTE;
using EnvDTE80;
using PinnacleCodingConvention.Options;
using System;

namespace PinnacleCodingConvention.Models.CodeItems
{
    /// <summary>
    /// The representation of a code field.
    /// </summary>
    internal class CodeItemField : BaseCodeItemElement
    {
        private readonly Lazy<bool> _isConstant;
        private readonly Lazy<bool> _isEnumItem;
        private readonly Lazy<bool> _isReadOnly;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeItemField" /> class.
        /// </summary>
        public CodeItemField()
        {
            _Access = LazyTryDefault(() => CodeVariable?.Access ?? vsCMAccess.vsCMAccessPublic);

            _Attributes = LazyTryDefault(() => CodeVariable?.Attributes);

            _DocComment = LazyTryDefault(() => CodeVariable?.DocComment);

            _isConstant = LazyTryDefault(() => CodeVariable != null && CodeVariable.IsConstant && CodeVariable.ConstKind == vsCMConstKind.vsCMConstKindConst);

            _isEnumItem = LazyTryDefault(() => CodeVariable?.Parent is CodeEnum);

            _isReadOnly = LazyTryDefault(() => CodeVariable != null && CodeVariable.IsConstant && CodeVariable.ConstKind == vsCMConstKind.vsCMConstKindReadOnly);

            _IsStatic = LazyTryDefault(() => CodeVariable != null && CodeVariable.IsShared);

            _TypeString = LazyTryDefault(() => CodeVariable?.Type?.AsString);
        }

        /// <summary>
        /// Gets the kind.
        /// </summary>
        public override KindCodeItem Kind => GeneralOptions.Instance.IsSeparateConstAndVar && IsConstant ? KindCodeItem.Constants : KindCodeItem.Field;

        /// <summary>
        /// Loads all lazy initialized values immediately.
        /// </summary>
        public override void LoadLazyInitializedValues()
        {
            base.LoadLazyInitializedValues();
            _ = IsConstant;
            _ = IsEnumItem;
            _ = IsReadOnly;
        }

        /// <summary>
        /// Gets or sets the underlying VSX CodeVariable.
        /// </summary>
        public CodeVariable2 CodeVariable { get; set; }

        /// <summary>
        /// Gets a flag indicating if this field is a constant.
        /// </summary>
        public bool IsConstant => _isConstant.Value;

        /// <summary>
        /// Gets a flag indicating if this field is an enumeration item.
        /// </summary>
        public bool IsEnumItem => _isEnumItem.Value;

        /// <summary>
        /// Gets a flag indicating if this field is read-only.
        /// </summary>
        public bool IsReadOnly => _isReadOnly.Value;

    }
}