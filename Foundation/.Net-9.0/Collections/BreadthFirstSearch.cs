using System;
using System.Collections.Generic;

namespace Foundation.Collections;

public static class BreadthFirstSearch
{
    public static void Search<TNode>(TNode node, Func<TNode, IEnumerable<TNode>> getChildNodes, Func<TNode, bool> visit)
    {
        ArgumentNullException.ThrowIfNull(getChildNodes);
        ArgumentNullException.ThrowIfNull(visit);

        Queue<TNode> queue = new Queue<TNode>();
        queue.Enqueue(node);

        while (queue.Count > 0)
        {
            TNode currentNode = queue.Dequeue();
            bool continueSearch = visit(currentNode);

            if (!continueSearch)
                break;

            IEnumerable<TNode> childNodes = getChildNodes(currentNode);

            foreach (TNode childNode in childNodes)
                queue.Enqueue(childNode);
        }
    }
}