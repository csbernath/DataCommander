using System;
using System.Collections.ObjectModel;

namespace DataCommander.Api;

public class MenuItem(string text, EventHandler onClick, ReadOnlyCollection<MenuItem> dropDownItems)
{
    public readonly string Text = text;
    public readonly EventHandler OnClick = onClick;
    public readonly ReadOnlyCollection<MenuItem> DropDownItems = dropDownItems;
}