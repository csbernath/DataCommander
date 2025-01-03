﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Foundation.Collections.ReadOnly;

namespace Foundation.Diagnostics;

public static class DotNetFrameworkVersionStore
{
    private static readonly ReadOnlySortedList<int, string> Items;

    static DotNetFrameworkVersionStore()
    {
        var items = new[]
        {
            KeyValuePair.Create(378389, "4.5"),
            KeyValuePair.Create(378675, "4.5.1 (server)"),
            KeyValuePair.Create(378758, "4.5.1 (client)"),
            KeyValuePair.Create(379893, "4.5.2"),
            KeyValuePair.Create(394254, "4.6.1 (Windows 10)"),
            KeyValuePair.Create(394271, "4.6.1"),
            KeyValuePair.Create(394802, "4.6.2 (Windows 10 Anniversary Update)"),
            KeyValuePair.Create(394806, "4.6.2"),
            KeyValuePair.Create(460798, "4.7 (Windows 10 Creators Update)"),
            KeyValuePair.Create(460805, "4.7"),
            KeyValuePair.Create(461308, "4.7.1 (On Windows 10 Fall Creators Update and Windows Server, version 1709)"),
            KeyValuePair.Create(461310, "4.7.1 (On all other Windows operating systems (including other Windows 10 operating systems)"),
            KeyValuePair.Create(461808, "4.7.2 (Windows 10 April 2018 Update)"),
            KeyValuePair.Create(461814, "4.7.2"),
            KeyValuePair.Create(528040, "4.8 (On Windows 10 May 2019 Update)"),
            KeyValuePair.Create(528049, "4.8 (On all others Windows operating systems (including other Windows 10 operating systems))"),
            KeyValuePair.Create(528372, "4.8 (On Windows 10 May 2020 Update and Windows 10 October 2020 Update)")
        };
        Items = new ReadOnlySortedList<int, string>(items, Comparer<int>.Default.Compare);
    }

    public static bool TryGet(int release, [MaybeNullWhen(false)] out string version) => Items.TryGetValue(release, out version);
}