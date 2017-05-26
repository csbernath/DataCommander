using System.Diagnostics;

namespace Foundation.Data.TextData
{
    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("Name = {" + nameof(Name) + "}")]
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