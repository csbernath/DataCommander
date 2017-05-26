#if FOUNDATION_3_5

namespace DataCommander.Foundation.Diagnostics
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public sealed class AssertMessageParameterCollection : ICollection<AssertMessageParameter>
    {
        private List<AssertMessageParameter> list;

        /// <summary>
        /// 
        /// </summary>
        public AssertMessageParameterCollection()
        {
            this.list = new List<AssertMessageParameter>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public AssertMessageParameterCollection( IEnumerable<AssertMessageParameter> parameters )
        {
            this.list = new List<AssertMessageParameter>( parameters );
        }

        #region ICollection<AssertMessageParameter> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add( AssertMessageParameter item )
        {
            this.list.Add( item );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Add( string name, object value )
        {
            var item = new AssertMessageParameter( name, value );
            this.Add( item );
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            this.list.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains( AssertMessageParameter item )
        {
            return this.list.Contains( item );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo( AssertMessageParameter[] array, int arrayIndex )
        {
            this.list.CopyTo( array, arrayIndex );
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get
            {
                return this.list.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove( AssertMessageParameter item )
        {
            return this.list.Remove( item );
        }

        #endregion

        #region IEnumerable<AssertMessageParameter> Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<AssertMessageParameter> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        #endregion
    }
}

#endif