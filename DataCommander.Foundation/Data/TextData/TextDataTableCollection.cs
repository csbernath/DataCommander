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

        int IList<TextDataTable>.IndexOf(TextDataTable item)
        {
            throw new NotImplementedException();
        }

        void IList<TextDataTable>.Insert(int index, TextDataTable item)
        {
            throw new NotImplementedException();
        }

        void IList<TextDataTable>.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        TextDataTable IList<TextDataTable>.this[int index]
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

        bool ICollection<TextDataTable>.Contains(TextDataTable item)
        {
            throw new NotImplementedException();
        }

        void ICollection<TextDataTable>.CopyTo(TextDataTable[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        int ICollection<TextDataTable>.Count
        {
            get { throw new NotImplementedException();}
        }

        bool ICollection<TextDataTable>.IsReadOnly
        {
            get { throw new NotImplementedException();}
        }

        bool ICollection<TextDataTable>.Remove(TextDataTable item)
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
