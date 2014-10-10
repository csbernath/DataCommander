namespace DataCommander.Foundation.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SegmentCollection<T> : ICollection<T>
    {
        private readonly Int32 segmentSize;
        private Int32 count;
        private Segment first;
        private Segment last;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="segmentSize"></param>
        public SegmentCollection( Int32 segmentSize )
        {
            Contract.Requires( segmentSize > 0 );

            this.segmentSize = segmentSize;
        }

        #region ICollection<T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add( T item )
        {
            if (this.count == 0)
            {
                this.first = new Segment( this.segmentSize );
                this.last = this.first;
            }

            if (this.last.Count == segmentSize)
            {
                var newSegment = new Segment( this.segmentSize );
                this.last.Next = newSegment;
                this.last = newSegment;
            }

            this.last.Add( item );
            this.count++;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            this.count = 0;
            this.first = null;
            this.last = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Boolean Contains( T item )
        {
            var comparer = EqualityComparer<T>.Default;
            Boolean contains = false;
            var segment = this.first;

            while (segment != null)
            {
                Int32 count = segment.Count;

                for (Int32 i = 0; i < count; i++)
                {
                    var current = segment[ i ];
                    if (comparer.Equals( current, item ))
                    {
                        contains = true;
                        break;
                    }
                }

                segment = segment.Next;
            }

            return contains;
        }

        void ICollection<T>.CopyTo( T[] array, Int32 arrayIndex )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 Count
        {
            get
            {
                return this.count;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        bool ICollection<T>.Remove( T item )
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            var segment = this.first;

            while (segment != null)
            {
                Int32 count = segment.Count;

                for (Int32 i = 0; i < count; i++)
                {
                    yield return segment[ i ];
                }

                segment = segment.Next;
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            var enumerable = (IEnumerable<T>) this;
            return enumerable.GetEnumerator();
        }

        #endregion

        private sealed class Segment
        {
            private readonly T[] array;
            private Int32 count;
            private Segment next;

            public Segment( Int32 size )
            {
                this.array = new T[size];
            }

            public Int32 Count
            {
                get
                {
                    return this.count;
                }
            }

            public T this[ Int32 index ]
            {
                get
                {
                    return this.array[ index ];
                }
            }

            public Segment Next
            {
                get
                {
                    return this.next;
                }

                set
                {
                    this.next = value;
                }
            }

            public void Add( T item )
            {
                this.array[ this.count ] = item;
                this.count++;
            }
        }
    }
}