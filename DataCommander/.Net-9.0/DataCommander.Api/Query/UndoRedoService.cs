using System;
using System.Collections.Generic;
using System.Linq;
using Foundation.Assertions;

namespace DataCommander.Api.Query;

public static class UndoRedoService
{
    public static void Do<T>(this UndoRedoState<T> undoRedoState, IEnumerable<T> items)
    {
        ArgumentNullException.ThrowIfNull(undoRedoState);
        ArgumentNullException.ThrowIfNull(items);

        int unprocessedItemCount = undoRedoState.GetUnprocessedItemCount();
        if (unprocessedItemCount > 0)
            undoRedoState.Items.RemoveRange(undoRedoState.ProcessedItemCount, unprocessedItemCount);

        List<T> itemList = items.ToList();
        undoRedoState.Items.AddRange(itemList);
        undoRedoState.ProcessedItemCount += itemList.Count;
    }

    public static void Undo<T>(this UndoRedoState<T> undoRedoState, int itemCount, Action<IReadOnlyList<T>> process)
    {
        ArgumentNullException.ThrowIfNull(undoRedoState);
        Assert.IsInRange(itemCount > 0);
        ArgumentNullException.ThrowIfNull(process);

        if (undoRedoState.ProcessedItemCount < itemCount)
            throw new InvalidOperationException("Nincs meg a megadott darabszámú visszavonható művelet.");

        int processedItemCount = undoRedoState.ProcessedItemCount - itemCount;
        List<T> items = undoRedoState.Items.Take(processedItemCount).ToList();
        process(items);
        undoRedoState.ProcessedItemCount = processedItemCount;
    }

    public static void Redo<T>(this UndoRedoState<T> undoRedoState, int itemCount, Action<IReadOnlyList<T>> process)
    {
        ArgumentNullException.ThrowIfNull(undoRedoState);
        Assert.IsInRange(itemCount > 0);
        ArgumentNullException.ThrowIfNull(process);

        int unprocessedItemCount = undoRedoState.GetUnprocessedItemCount();
        if (unprocessedItemCount < itemCount)
            throw new InvalidOperationException("Nincs meg a megadott számú ismételhető művelet.");

        List<T> items = undoRedoState.Items.GetRange(undoRedoState.ProcessedItemCount, itemCount).ToList();
        process(items);
        undoRedoState.ProcessedItemCount += itemCount;
    }

    private static int GetUnprocessedItemCount<T>(this UndoRedoState<T> undoRedoState)
    {
        ArgumentNullException.ThrowIfNull(undoRedoState);

        return undoRedoState.Items.Count - undoRedoState.ProcessedItemCount;
    }
}