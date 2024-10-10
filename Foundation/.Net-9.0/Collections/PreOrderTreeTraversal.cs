using System;
using System.Collections.Generic;

namespace Foundation.Collections;

public static class PreOrderTreeTraversal
{
    public static void ForEach<T>(T rootNode, Func<T, IEnumerable<T>> getChildNodes, Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(getChildNodes);
        ArgumentNullException.ThrowIfNull(action);

        action(rootNode);

        foreach (var childNode in getChildNodes(rootNode))
            ForEach(childNode, getChildNodes, action);
    }

    public static T? FirstOrDefault<T>(T rootNode, Func<T, IEnumerable<T>> getChildNodes, Func<T, bool> predicate) where T : class
    {
        ArgumentNullException.ThrowIfNull(rootNode);
        ArgumentNullException.ThrowIfNull(getChildNodes);
        ArgumentNullException.ThrowIfNull(predicate);

        T? firstOrDefault = null;

        if (predicate(rootNode))
            firstOrDefault = rootNode;
        else
            foreach (var childNode in getChildNodes(rootNode))
            {
                firstOrDefault = FirstOrDefault(childNode, getChildNodes, predicate);
                if (firstOrDefault != null)
                    break;
            }

        return firstOrDefault;
    }
}