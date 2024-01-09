using System.Collections.ObjectModel;

namespace DataCommander.Api;

public class ContextMenu(ReadOnlyCollection<MenuItem> menuItems)
{
    public readonly ReadOnlyCollection<MenuItem> MenuItems = menuItems;
}