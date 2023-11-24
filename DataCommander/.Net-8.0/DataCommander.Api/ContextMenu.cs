using System.Collections.ObjectModel;

namespace DataCommander.Api;

public class ContextMenu
{
    public readonly ReadOnlyCollection<MenuItem> MenuItems;

    public ContextMenu(ReadOnlyCollection<MenuItem> menuItems)
    {
        MenuItems = menuItems;
    }
}