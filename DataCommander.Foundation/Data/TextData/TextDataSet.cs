namespace DataCommander.Foundation.Data
{
    using System.Diagnostics;

    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("Name = {Name}")]
    public sealed class TextDataSet
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public TextDataSet(string name)
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
        public TextDataSetTableCollection Tables { get; } = new TextDataSetTableCollection();
    }
}