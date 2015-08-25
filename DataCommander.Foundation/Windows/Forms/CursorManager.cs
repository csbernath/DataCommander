namespace DataCommander.Foundation.Windows.Forms
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Windows.Forms;

    /// <summary>
    /// 
    /// </summary>
    public sealed class CursorManager : IDisposable
    {
        private readonly Cursor originalCursor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cursor"></param>
        public CursorManager(Cursor cursor)
        {
            Contract.Requires<ArgumentNullException>(cursor != null);

            this.originalCursor = Cursor.Current;
            Cursor.Current = cursor;
        }

        /// <summary>
        /// 
        /// </summary>
        void IDisposable.Dispose()
        {
            Cursor.Current = this.originalCursor;
        }
    }
}