using EnvDTE;
using EnvDTE80;
using PinnacleCodingConvention.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PinnacleCodingConvention.Models.CodeItems
{
    /// <summary>
    /// The representation of a code method.
    /// </summary>
    internal class CodeItemMethod : BaseCodeItemElement, ICodeItemComplexity, ICodeItemParameters, IInterfaceItem
    {
        private readonly Lazy<int> _complexity;
        private readonly Lazy<bool> _isConstructor;
        private readonly Lazy<bool> _isDestructor;
        private readonly Lazy<bool> _isTestMethod;
        private readonly Lazy<bool> _isExplicitInterfaceImplementation;
        private readonly Lazy<vsCMOverrideKind> _overrideKind;
        private readonly Lazy<IEnumerable<CodeParameter>> _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeItemMethod" /> class.
        /// </summary>
        public CodeItemMethod()
        {
            // Make exceptions for static constructors and explicit interface implementations -
            // which report private access but really do not have a meaningful access level.
            _Access = LazyTryDefault(() => CodeFunction != null && !(IsStatic && IsConstructor) && !IsExplicitInterfaceImplementation ? CodeFunction.Access : vsCMAccess.vsCMAccessPublic);

            _Attributes = LazyTryDefault(() => CodeFunction?.Attributes);

            _complexity = LazyTryDefault(() => CodeElementHelper.CalculateComplexity(CodeElement));

            _DocComment = LazyTryDefault(() => CodeFunction?.DocComment);

            _isConstructor = LazyTryDefault(() => CodeFunction != null && CodeFunction.FunctionKind == vsCMFunction.vsCMFunctionConstructor);

            _isDestructor = LazyTryDefault(() => CodeFunction != null && CodeFunction.FunctionKind == vsCMFunction.vsCMFunctionDestructor);

            _isExplicitInterfaceImplementation = LazyTryDefault(() => CodeFunction != null && ExplicitInterfaceImplementationHelper.IsExplicitInterfaceImplementation(CodeFunction));

            _IsStatic = LazyTryDefault(() => CodeFunction != null && CodeFunction.IsShared);

            _overrideKind = LazyTryDefault(() => CodeFunction?.OverrideKind ?? vsCMOverrideKind.vsCMOverrideKindNone);

            _parameters = LazyTryDefault(() => CodeFunction?.Parameters?.Cast<CodeParameter>().ToList() ?? Enumerable.Empty<CodeParameter>());

            _TypeString = LazyTryDefault(() => CodeFunction?.Type?.AsString);

            _isTestMethod = LazyTryDefault(() => Attributes.OfType<CodeAttribute>().Any(attribute => attribute.Name == "TestMethod"));
        }

        /// <summary>
        /// Gets the kind.
        /// </summary>
        public override KindCodeItem Kind
        {
            get
            {
                if (IsConstructor)
                {
                    return KindCodeItem.Constructor;
                }

                if (IsDestructor)
                {
                    return KindCodeItem.Destructor;
                }

                if (IsTestMethod)
                {
                    return KindCodeItem.TestMethod;
                }

                return KindCodeItem.Method;
            }
        }

        /// <summary>
        /// Loads all lazy initialized values immediately.
        /// </summary>
        public override void LoadLazyInitializedValues()
        {
            base.LoadLazyInitializedValues();
            _ = Complexity;
            _ = IsConstructor;
            _ = IsDestructor;
            _ = IsExplicitInterfaceImplementation;
            _ = OverrideKind;
            _ = Parameters;
        }

        /// <summary>
        /// Gets or sets the underlying VSX CodeFunction.
        /// </summary>
        public CodeFunction2 CodeFunction { get; set; }

        /// <summary>
        /// Gets the complexity.
        /// </summary>
        public int Complexity => _complexity.Value;

        /// <summary>
        /// Gets a flag indicating if this method is a constructor.
        /// </summary>
        public bool IsConstructor => _isConstructor.Value;

        /// <summary>
        /// Gets a flag indicating if this method is a destructor.
        /// </summary>
        public bool IsDestructor => _isDestructor.Value;

        /// <summary>
        /// Gets a flag indicating if this method is a test method.
        /// </summary>
        public bool IsTestMethod => _isTestMethod.Value;

        /// <summary>
        /// Gets a flag indicating if this method is an explicit interface implementation.
        /// </summary>
        public bool IsExplicitInterfaceImplementation => _isExplicitInterfaceImplementation.Value;

        /// <summary>
        /// Gets the override kind (abstract, virtual, override, new), defaulting to none.
        /// </summary>
        public vsCMOverrideKind OverrideKind => _overrideKind.Value;

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        public IEnumerable<CodeParameter> Parameters => _parameters.Value;

    }
}