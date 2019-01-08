using System.Collections.Generic;

namespace DataCommander.Providers.Query
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