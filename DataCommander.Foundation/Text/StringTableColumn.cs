namespace DataCommander.Foundation.Text
{
    using System;

	/// <summary>
    /// 
    /// </summary>
    public enum StringTableColumnAlign
    {
        /// <summary>
        /// 
        /// </summary>
        Left,

        /// <summary>
        /// 
        /// </summary>
        Right
    }

    /// <summary>
    /// 
    /// </summary>
    public class StringTableColumn
    {
        private StringTableColumnAlign align = StringTableColumnAlign.Left;
        private Int32 width;

        internal StringTableColumn()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public StringTableColumnAlign Align
        {
            get
            {
                return this.align;
            }

            set
            {
                this.align = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 Width
        {
            get
            {
                return this.width;
            }

            internal set
            {
                this.width = value;
            }
        }
    }
}