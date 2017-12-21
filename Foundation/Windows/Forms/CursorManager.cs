﻿using System;
using System.Windows.Forms;

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
#if CONTRACTS_FULL
            FoundationContract.Requires<ArgumentNullException>(cursor != null);
#endif

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