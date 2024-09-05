using System;

namespace DataCommander.Api.Query;

public sealed class ListBoxItem<T>(T item, Func<T, string> toString)
{
    public readonly T Item = item;

    public override string ToString() => toString(Item);
}