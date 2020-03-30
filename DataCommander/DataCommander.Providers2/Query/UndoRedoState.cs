using System.Collections.Generic;

namespace DataCommander.Providers2.Query
{
    public class UndoRedoState<T>
    {
        public List<T> Items;
        public int ProcessedItemCount;

        public UndoRedoState(List<T> items, int processedItemCount)
        {
            Items = items;
            ProcessedItemCount = processedItemCount;
        }
    }
}