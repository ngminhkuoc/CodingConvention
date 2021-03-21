using System.Collections.Generic;
using CodingConvention.Models;
using CodingConvention.Models.CodeItems;
using EnvDTE;

namespace CodingConvention.Helpers
{
    /// <summary>
    /// A helper for comparing code items by type, access level, etc.
    /// </summary>
    internal class CodeItemTypeComparer : Comparer<BaseCodeItem>
    {
        private readonly bool _secondaryOrderByName;
        private readonly List<KindCodeItem> _kindOrder = new List<KindCodeItem>
        {
            KindCodeItem.Constants,
            KindCodeItem.Field,
            KindCodeItem.Constructor,
            KindCodeItem.Method,
            KindCodeItem.TestMethod,
            KindCodeItem.Property,
            KindCodeItem.Destructor,
        };
        private readonly List<AccessModifier> _accessModifierOrder = new List<AccessModifier>
        {
            AccessModifier.Public,
            AccessModifier.ProtectedInternal,
            AccessModifier.Protected,
            AccessModifier.Internal,
            AccessModifier.Default,
            AccessModifier.PrivateProtected,
            AccessModifier.Private,
        };

        internal CodeItemTypeComparer() : this(true) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeItemTypeComparer"/> class.
        /// </summary>
        /// <param name="secondaryOrderByName">Determines whether a secondary sort by name is performed or not.</param>
        internal CodeItemTypeComparer(bool secondaryOrderByName)
        {
            _secondaryOrderByName = secondaryOrderByName;
        }

        /// <summary>
        /// Performs a comparison of two objects of the same type and returns a value indicating
        /// whether one object is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// Less than zero: <paramref name="x" /> is less than <paramref name="y" />.
        /// Zero: <paramref name="x" /> equals <paramref name="y" />.
        /// Greater than zero: <paramref name="x" /> is greater than <paramref name="y" />.
        /// </returns>
        public override int Compare(BaseCodeItem x, BaseCodeItem y)
        {
            int first = CalculateNumericRepresentation(x);
            int second = CalculateNumericRepresentation(y);
            return first.CompareTo(second);
        }

        private int CalculateNumericRepresentation(BaseCodeItem codeItem)
        {
            int typeOffset = CalculateTypeOffset(codeItem);
            int constantOffset = CalculateConstantOffset(codeItem);
            int readOnlyOffset = CalculateReadOnlyOffset(codeItem);
            int accessModifierOffset = CalculateAccessModifierOffset(codeItem);

            int calc = typeOffset * 1000 + constantOffset * 100 + readOnlyOffset * 10 + accessModifierOffset;

            return calc;
        }

        private int CalculateAccessModifierOffset(BaseCodeItem codeItem)
        {
            var codeItemElement = codeItem as BaseCodeItemElement;
            if (codeItemElement == null)
                return 0;

            return _accessModifierOrder.IndexOf(Map(codeItemElement.Access)) + 1;
        }

        private AccessModifier Map(vsCMAccess access)
        {
            switch (access)
            {
                case vsCMAccess.vsCMAccessPublic:
                    return AccessModifier.Public;
                case vsCMAccess.vsCMAccessProject:
                    return AccessModifier.Internal;
                case vsCMAccess.vsCMAccessProtected:
                    return AccessModifier.Protected;
                case vsCMAccess.vsCMAccessDefault:
                    return AccessModifier.Default;
                case vsCMAccess.vsCMAccessProjectOrProtected:
                    return AccessModifier.ProtectedInternal;
                default:
                    return AccessModifier.Private;
            }
            //there is no mapping for private protected
        }

        private int CalculateTypeOffset(BaseCodeItem codeItem)
        {
            return _kindOrder.IndexOf(codeItem.Kind) + 1;
        }

        private static int CalculateConstantOffset(BaseCodeItem codeItem)
        {
            var codeItemField = codeItem as CodeItemField;
            if (codeItemField == null)
                return 0;

            return codeItemField.IsConstant ? 0 : 1;
        }

        private static int CalculateReadOnlyOffset(BaseCodeItem codeItem)
        {
            return codeItem is CodeItemField codeItemField && codeItemField.IsReadOnly ? 0 : 1;
        }

    }
}
