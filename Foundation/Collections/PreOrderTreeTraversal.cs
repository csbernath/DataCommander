using System;
using System.Collections.Generic;
using Foundation.Diagnostics;
using Foundation.Diagnostics.Assertions;

namespace Foundation.Collections
{
    /// <summary>
    /// 
    /// </summary>
    public static class PreOrderTreeTraversal
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rootNode"></param>
        /// <param name="getChildNodes"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(T rootNode, Func<T, IEnumerable<T>> getChildNodes, Action<T> action)
        {
            Assert.IsNotNull(getChildNodes);
            Assert.IsNotNull(action);

            action(rootNode);

            foreach (var childNode in getChildNodes(rootNode))
                ForEach(childNode, getChildNodes, action);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rootNode"></param>
        /// <param name="getChildNodes"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static T FirstOrDefault<T>(T rootNode, Func<T, IEnumerable<T>> getChildNodes, Func<T, bool> predicate) where T : class
        {
            Assert.IsNotNull(rootNode);
            Assert.IsNotNull(getChildNodes);
            Assert.IsNotNull(predicate);

            T firstOrDefault = null;

            if (predicate(rootNode))
                firstOrDefault = rootNode;
            else
            {
                foreach (var childNode in getChildNodes(rootNode))
                {
                    firstOrDefault = FirstOrDefault<T>(childNode, getChildNodes, predicate);
                    if (firstOrDefault != null)
                        break;
                }
            }

            return firstOrDefault;
        }
    }
}