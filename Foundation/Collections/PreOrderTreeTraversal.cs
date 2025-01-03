using System;
using System.Collections.Generic;
using Foundation.Assertions;

namespace Foundation.Collections;

public static class PreOrderTreeTraversal
{
    public static void ForEach<T>(T rootNode, Func<T, IEnumerable<T>> getChildNodes, Action<T> action)
    {
        Assert.IsNotNull(getChildNodes);
        Assert.IsNotNull(action);

        action(rootNode);

        foreach (var childNode in getChildNodes(rootNode))
            ForEach(childNode, getChildNodes, action);
    }

    public static T FirstOrDefault<T>(T rootNode, Func<T, IEnumerable<T>> getChildNodes, Func<T, bool> predicate) where T : class
    {
        Assert.IsNotNull(rootNode);
        Assert.IsNotNull(getChildNodes);
        Assert.IsNotNull(predicate);

        T firstOrDefault = null;

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