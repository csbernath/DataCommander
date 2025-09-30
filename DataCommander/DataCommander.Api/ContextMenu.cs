using System.Collections.Generic;

namespace DataCommander.Api;

public class ContextMenu(IReadOnlyCollection<MenuItem> menuItems)
{
    public readonly IReadOnlyCollection<MenuItem> MenuItems = menuItems;
}