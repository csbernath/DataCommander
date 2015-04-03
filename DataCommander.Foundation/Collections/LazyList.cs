namespace DataCommander.Foundation.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LazyList<T> : LazyCollection<T>, IList<T>
    {
        #region Private Fields

        private readonly Func<IList<T>> createList;
        private IList<T> list;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createList"></param>
        public LazyList(Func<IList<T>> createList)
        {
            Contract.Requires(createList != null);

            this.createList = createList;
            this.Initialize(this.GetCollection);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        public IList<T> InnerList
        {
            get
            {
                return this.list;
            }
        }

        #endregion

        #region IList<T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                T item;

                if (this.list != null)
                {
                    item = this.list[index];
                }
                else
                {
                    throw new ArgumentOutOfRangeException("index");
                }

                return item;
            }

            set
            {
                if (this.list != null)
                {
                    this.list[index] = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("index");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(T item)
        {
            int index;

            if (this.list != null)
            {
                index = this.list.IndexOf(item);
            }
            else
            {
                index = -1;
            }

            return index;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, T item)
        {
            this.GetList();
            this.list.Insert(index, item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            if (this.list != null)
            {
                this.list.RemoveAt(index);
            }
            else
            {
                throw new ArgumentOutOfRangeException("index");
            }
        }

        #endregion

        #region Private Methods

        private void GetList()
        {
            if (this.list == null)
            {
                this.list = this.createList();
                Contract.Assert(this.list != null);
            }
        }

        private ICollection<T> GetCollection()
        {
            this.GetList();
            return this.list;
        }

        #endregion
    }
}