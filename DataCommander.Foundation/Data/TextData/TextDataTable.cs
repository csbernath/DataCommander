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
        private readonly String name;
        private TextDataColumnCollection columns = new TextDataColumnCollection();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public TextDataTable(String name)
        {
            this.name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        public String Name
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
