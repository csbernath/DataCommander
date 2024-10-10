using System;
using System.Windows.Forms;

namespace Foundation.Windows.Forms;

public sealed class CursorManager : IDisposable
{
    private readonly Cursor? _originalCursor;

    public CursorManager(Cursor cursor)
    {
        ArgumentNullException.ThrowIfNull(cursor);

        _originalCursor = Cursor.Current;
        Cursor.Current = cursor;
    }

    void IDisposable.Dispose() => Cursor.Current = _originalCursor;
}