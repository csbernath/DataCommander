using System;
using System.Windows.Forms;
using Foundation.Diagnostics.Assertions;

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
            Assert.IsNotNull(cursor);

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