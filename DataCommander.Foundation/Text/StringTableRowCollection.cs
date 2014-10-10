namespace DataCommander.Foundation.Text
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    public class StringTableRowCollection : Collection<StringTableRow>
    {
        internal StringTableRowCollection()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void InsertItem( Int32 index, StringTableRow item )
        {
            Contract.Assert( item != null );

            base.InsertItem( index, item );
        }
    }
}