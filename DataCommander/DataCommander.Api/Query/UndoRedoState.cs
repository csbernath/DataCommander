using System.Collections.Generic;

namespace DataCommander.Api.Query;

public class UndoRedoState<T>(List<T> items, int processedItemCount)
{
    public List<T> Items = items;
    public int ProcessedItemCount = processedItemCount;
}