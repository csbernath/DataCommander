using System;

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
        int selectedIndex = -1;

        for (int i = 0; i < _selections.Length; ++i)
        {
            Func<TArgument, bool> selection = _selections[i];
            bool selected = selection(argument);
            if (selected)
            {
                selectedIndex = i;
                break;
            }
        }

        return selectedIndex;
    }
}