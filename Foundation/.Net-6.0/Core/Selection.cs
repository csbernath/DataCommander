using System;
using Foundation.Assertions;

namespace Foundation.Core;

public sealed class MultipleDispatchSelection<TArgument>
{
    private readonly Func<TArgument, bool>[] _selections;

    public MultipleDispatchSelection(params Func<TArgument, bool>[] selections)
    {
        ArgumentNullException.ThrowIfNull(selections);
        _selections = selections;
    }

    public int Select(TArgument argument)
    {
        var selectedIndex = -1;

        for (var i = 0; i < _selections.Length; ++i)
        {
            var selection = _selections[i];
            var selected = selection(argument);
            if (selected)
            {
                selectedIndex = i;
                break;
            }
        }

        return selectedIndex;
    }
}