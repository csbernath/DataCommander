namespace DataCommander.Foundation.Data
{
    using System.Diagnostics;

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
            this.Name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 
        /// </summary>
        public TextDataColumnCollection Columns { get; } = new TextDataColumnCollection();
    }
}
