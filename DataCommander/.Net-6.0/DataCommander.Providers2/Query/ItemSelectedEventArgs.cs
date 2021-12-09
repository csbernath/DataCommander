using System;

namespace DataCommander.Providers2.Query;

public sealed class ItemSelectedEventArgs : EventArgs
{
    public ItemSelectedEventArgs(int startIndex, int length, IObjectName objectName)
    {
        StartIndex = startIndex;
        Length = length;
        ObjectName = objectName;
    }

    public int StartIndex { get; }

    public int Length { get; }

    public IObjectName ObjectName { get; }
}