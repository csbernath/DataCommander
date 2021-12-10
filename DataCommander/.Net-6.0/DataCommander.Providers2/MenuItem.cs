using System;
using System.Collections.ObjectModel;

namespace DataCommander.Providers2;

public class MenuItem
{
    public readonly string Text;
    public readonly EventHandler OnClick;
    public readonly ReadOnlyCollection<MenuItem> DropDownItems;

    public MenuItem(string text, EventHandler onClick, ReadOnlyCollection<MenuItem> dropDownItems)
    {
        Text = text;
        OnClick = onClick;
        DropDownItems = dropDownItems;
    }
}