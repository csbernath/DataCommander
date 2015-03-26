namespace DataCommander.Foundation.Data
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("Name = {name}")]
    public sealed class TextDataTable
    {
        private readonly string name;
        private TextDataColumnCollection columns = new TextDataColumnCollection();

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
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public TextDataColumnCollection Columns
        {
            get
            {
                return this.columns;
            }
        }
    }
}
