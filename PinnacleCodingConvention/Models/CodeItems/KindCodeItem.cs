using System.ComponentModel;

namespace PinnacleCodingConvention.Models.CodeItems
{
    /// <summary>
    /// An enumeration of kind of code items. Does not 1:1 correspond to the code item classes (ex:
    /// CodeItemMethod includes constructors and destructors).
    /// </summary>
    internal enum KindCodeItem
    {
        [Description("Constants")]
        Constants,

        [Description("Fields")]
        Field,

        [Description("Constructors")]
        Constructor,

        [Description("Destructors")]
        Destructor,

        [Description("Delegates")]
        Delegate,

        [Description("Events")]
        Event,

        [Description("Enums")]
        Enum,

        [Description("Indexers")]
        Indexer,

        [Description("Interfaces")]
        Interface,

        [Description("Properties")]
        Property,

        [Description("Methods")]
        Method,

        [Description("Structs")]
        Struct,

        [Description("Classes")]
        Class,

        [Description("Namespaces")]
        Namespace,

        [Description("Regions")]
        Region,

        [Description("Usings")]
        Using
    }
}