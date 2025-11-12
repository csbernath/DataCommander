using System;
using System.Collections.Generic;
using System.Linq;
using Foundation.Assertions;

namespace DataCommander.Api.Query;

public static class UndoRedoService
{
    extension<T>(UndoRedoState<T> undoRedoState)
    {
        public void Do(IEnumerable<T> items)
        {
            ArgumentNullException.ThrowIfNull(undoRedoState);
            ArgumentNullException.ThrowIfNull(items);

            var unprocessedItemCount = undoRedoState.GetUnprocessedItemCount();
            if (unprocessedItemCount > 0)
                undoRedoState.Items.RemoveRange(undoRedoState.ProcessedItemCount, unprocessedItemCount);

            var itemList = items.ToList();
            undoRedoState.Items.AddRange(itemList);
            undoRedoState.ProcessedItemCount += itemList.Count;
        }

        public void Undo(int itemCount, Action<IReadOnlyList<T>> process)
        {
            ArgumentNullException.ThrowIfNull(undoRedoState);
            Assert.IsInRange(itemCount > 0);
            ArgumentNullException.ThrowIfNull(process);

            if (undoRedoState.ProcessedItemCount < itemCount)
                throw new InvalidOperationException("Nincs meg a megadott darabszámú visszavonható művelet.");

            var processedItemCount = undoRedoState.ProcessedItemCount - itemCount;
            var items = undoRedoState.Items.Take(processedItemCount).ToList();
            process(items);
            undoRedoState.ProcessedItemCount = processedItemCount;
        }

        public void Redo(int itemCount, Action<IReadOnlyList<T>> process)
        {
            ArgumentNullException.ThrowIfNull(undoRedoState);
            Assert.IsInRange(itemCount > 0);
            ArgumentNullException.ThrowIfNull(process);

            var unprocessedItemCount = undoRedoState.GetUnprocessedItemCount();
            if (unprocessedItemCount < itemCount)
                throw new InvalidOperationException("Nincs meg a megadott számú ismételhető művelet.");

            var items = undoRedoState.Items.GetRange(undoRedoState.ProcessedItemCount, itemCount).ToList();
            process(items);
            undoRedoState.ProcessedItemCount += itemCount;
        }

        private int GetUnprocessedItemCount()
        {
            ArgumentNullException.ThrowIfNull(undoRedoState);

            return undoRedoState.Items.Count - undoRedoState.ProcessedItemCount;
        }
    }
}