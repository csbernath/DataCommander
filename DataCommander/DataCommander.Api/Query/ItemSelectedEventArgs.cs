using System;

namespace DataCommander.Api.Query;

public sealed class ItemSelectedEventArgs(int startIndex, int length, IObjectName objectName) : EventArgs
{
    public int StartIndex { get; } = startIndex;

    public int Length { get; } = length;

    public IObjectName ObjectName { get; } = objectName;
}