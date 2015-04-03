namespace DataCommander.Foundation.Text
{
    /// <summary>
    /// 
    /// </summary>
    public class StringTableColumn
    {
        private StringTableColumnAlign align = StringTableColumnAlign.Left;
        private int width;

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
        public int Width
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