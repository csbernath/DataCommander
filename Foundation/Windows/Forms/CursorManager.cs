using System;
using System.Windows.Forms;
using Foundation.Diagnostics.Contracts;

namespace Foundation.Windows.Forms
{
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
            FoundationContract.Requires<ArgumentNullException>(cursor != null);

            originalCursor = Cursor.Current;
            Cursor.Current = cursor;
        }

        /// <summary>
        /// 
        /// </summary>
        void IDisposable.Dispose()
        {
            Cursor.Current = originalCursor;
        }
    }
}