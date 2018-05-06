using System;
using Foundation.Assertions;

namespace Foundation.Collections
{
    public static class LinearSearch
    {
        public static int IndexOf(int minIndex, int maxIndex, Func<int, bool> predicate)
        {
            Assert.IsNotNull(predicate);

            var index = -1;

            while (minIndex <= maxIndex)
            {
                if (predicate(minIndex))
                {
                    index = minIndex;
                    break;
                }

                minIndex++;
            }

            return index;
        }

        public static int LastIndexOf(int minIndex, int maxIndex, Func<int, bool> predicate)
        {
            Assert.IsNotNull(predicate);

            var index = -1;

            while (minIndex <= maxIndex)
            {
                if (predicate(maxIndex))
                {
                    index = maxIndex;
                    break;
                }

                maxIndex--;
            }

            return index;
        }
    }
}