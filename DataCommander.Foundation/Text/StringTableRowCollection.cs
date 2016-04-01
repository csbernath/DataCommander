namespace DataCommander.Foundation.Text
{
    using System.Collections;
    using System.Collections.Generic;
    using DataCommander.Foundation.Collections;

    /// <summary>
    /// 
    /// </summary>
    public class StringTableRowCollection : IEnumerable<StringTableRow>
    {
        private readonly SegmentedCollection<StringTableRow> rows = new SegmentedCollection<StringTableRow>(64);

        internal StringTableRowCollection()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count => this.rows.Count;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        public void Add(StringTableRow row)
        {
            this.rows.Add(row);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<StringTableRow> GetEnumerator()
        {
            IEnumerable<StringTableRow> enumerable = this.rows;
            return enumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}