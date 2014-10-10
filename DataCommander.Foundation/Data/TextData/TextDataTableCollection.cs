namespace DataCommander.Foundation.Data
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public sealed class TextDataTableList : IList<TextDataTable>
    {
        #region IList<TextDataTable> Members

        Int32 IList<TextDataTable>.IndexOf(TextDataTable item)
        {
            throw new NotImplementedException();
        }

        void IList<TextDataTable>.Insert(Int32 index, TextDataTable item)
        {
            throw new NotImplementedException();
        }

        void IList<TextDataTable>.RemoveAt(Int32 index)
        {
            throw new NotImplementedException();
        }

        TextDataTable IList<TextDataTable>.this[Int32 index]
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region ICollection<TextDataTable> Members

        void ICollection<TextDataTable>.Add(TextDataTable item)
        {
            throw new NotImplementedException();
        }

        void ICollection<TextDataTable>.Clear()
        {
            throw new NotImplementedException();
        }

        Boolean ICollection<TextDataTable>.Contains(TextDataTable item)
        {
            throw new NotImplementedException();
        }

        void ICollection<TextDataTable>.CopyTo(TextDataTable[] array, Int32 arrayIndex)
        {
            throw new NotImplementedException();
        }

        Int32 ICollection<TextDataTable>.Count
        {
            get { throw new NotImplementedException();}
        }

        Boolean ICollection<TextDataTable>.IsReadOnly
        {
            get { throw new NotImplementedException();}
        }

        Boolean ICollection<TextDataTable>.Remove(TextDataTable item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<TextDataTable> Members

        IEnumerator<TextDataTable> IEnumerable<TextDataTable>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
