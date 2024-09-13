using System;
using System.Collections.Generic;

namespace DataCommander.Api;

public class MenuItem(string text, EventHandler onClick, IReadOnlyCollection<MenuItem> dropDownItems)
{
    public readonly string Text = text;
    public readonly EventHandler OnClick = onClick;
    public readonly IReadOnlyCollection<MenuItem> DropDownItems = dropDownItems;
}