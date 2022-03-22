using System;
using System.Collections.Generic;
using Foundation.Assertions;

namespace Foundation.Collections;

public static class BreadthFirstSearch
{
    public static void Search<TNode>(TNode node, Func<TNode, IEnumerable<TNode>> getChildNodes, Func<TNode, bool> visit)
    {
        Assert.IsNotNull(getChildNodes);
        Assert.IsNotNull(visit);

        var queue = new Queue<TNode>();
        queue.Enqueue(node);

        while (queue.Count > 0)
        {
            var currentNode = queue.Dequeue();
            var continueSearch = visit(currentNode);

            if (!continueSearch)
                break;

            var childNodes = getChildNodes(currentNode);

            foreach (var childNode in childNodes)
                queue.Enqueue(childNode);
        }
    }
}