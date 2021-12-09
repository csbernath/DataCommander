using System.Diagnostics;

namespace Foundation.Data.TextData
{
    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("Name = {" + nameof(Name) + "}")]
    public sealed class TextDataTable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public TextDataTable(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 
        /// </summary>
        public TextDataColumnCollection Columns { get; } = new();
    }
}
