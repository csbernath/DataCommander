using System.Collections;
using System.Collections.Generic;
using Foundation.Collections;

namespace Foundation.Text
{
    /// <summary>
    /// 
    /// </summary>
    public class StringTableRowCollection : IEnumerable<StringTableRow>
    {
        private readonly SegmentedCollection<StringTableRow> _rows = new SegmentedCollection<StringTableRow>(64);

        internal StringTableRowCollection()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count => _rows.Count;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        public void Add(StringTableRow row)
        {
            _rows.Add(row);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<StringTableRow> GetEnumerator()
        {
            IEnumerable<StringTableRow> enumerable = _rows;
            return enumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}