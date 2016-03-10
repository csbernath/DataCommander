namespace DataCommander.Foundation.Data
{
    using System.Diagnostics;

    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("Name = {name}")]
    public sealed class TextDataTable
    {
        private readonly string name;
        private readonly TextDataColumnCollection columns = new TextDataColumnCollection();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public TextDataTable(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name => this.name;

        /// <summary>
        /// 
        /// </summary>
        public TextDataColumnCollection Columns => this.columns;
    }
}
