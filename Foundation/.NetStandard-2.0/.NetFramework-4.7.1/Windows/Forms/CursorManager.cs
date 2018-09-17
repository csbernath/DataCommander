using System;
using System.Windows.Forms;
using Foundation.Assertions;

namespace Foundation.Windows.Forms
{
    public sealed class CursorManager : IDisposable
    {
        private readonly Cursor originalCursor;

        public CursorManager(Cursor cursor)
        {
            Assert.IsNotNull(cursor);

            originalCursor = Cursor.Current;
            Cursor.Current = cursor;
        }

        void IDisposable.Dispose() => Cursor.Current = originalCursor;
    }
}