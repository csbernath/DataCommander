using System.Collections.Generic;
using Foundation.Collections;
using Foundation.Collections.ReadOnly;

namespace Foundation.Diagnostics
{
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
                KeyValuePair.Create(461308, "4.7.1 (Windows 10 Fall Creators Update 1709)"),
                KeyValuePair.Create(461808, "4.7.2 (Windows 10 April 2018 Update)"),
                KeyValuePair.Create(461814, "4.7.2")
            };
            Items = new ReadOnlySortedList<int, string>(items, Comparer<int>.Default.Compare);
        }

        public static bool TryGet(int release, out string version) => Items.TryGetValue(release, out version);
    }
}