using System;
using System.Collections.Generic;
using System.Linq;
using Foundation.Assertions;

namespace DataCommander.Providers.Query
{
    public static class UndoRedoService
    {
        public static void Do<T>(this UndoRedoState<T> undoRedoState, IEnumerable<T> items)
        {
            Assert.IsNotNull(undoRedoState);
            Assert.IsNotNull(items);

            var unprocessedItemCount = undoRedoState.GetUnprocessedItemCount();
            if (unprocessedItemCount > 0)
                undoRedoState.Items.RemoveRange(undoRedoState.ProcessedItemCount, unprocessedItemCount);

            var itemList = items.ToList();
            undoRedoState.Items.AddRange(itemList);
            undoRedoState.ProcessedItemCount += itemList.Count;
        }

        public static void Undo<T>(this UndoRedoState<T> undoRedoState, int itemCount, Action<IReadOnlyList<T>> process)
        {
            Assert.IsNotNull(undoRedoState);
            Assert.IsInRange(itemCount > 0);
            Assert.IsNotNull(process);

            if (undoRedoState.ProcessedItemCount < itemCount)
                throw new InvalidOperationException("Nincs meg a megadott darabszámú visszavonható művelet.");

            var processedItemCount = undoRedoState.ProcessedItemCount - itemCount;
            var items = undoRedoState.Items.Take(processedItemCount).ToList();
            process(items);
            undoRedoState.ProcessedItemCount = processedItemCount;
        }

        public static void Redo<T>(this UndoRedoState<T> undoRedoState, int itemCount, Action<IReadOnlyList<T>> process)
        {
            Assert.IsNotNull(undoRedoState);
            Assert.IsInRange(itemCount > 0);
            Assert.IsNotNull(process);

            var unprocessedItemCount = undoRedoState.GetUnprocessedItemCount();
            if (unprocessedItemCount < itemCount)
                throw new InvalidOperationException("Nincs meg a megadott számú ismételhető művelet.");

            var items = undoRedoState.Items.GetRange(undoRedoState.ProcessedItemCount, itemCount).ToList();
            process(items);
            undoRedoState.ProcessedItemCount += itemCount;
        }

        private static int GetUnprocessedItemCount<T>(this UndoRedoState<T> undoRedoState)
        {
            Assert.IsNotNull(undoRedoState);

            return undoRedoState.Items.Count - undoRedoState.ProcessedItemCount;
        }
    }
}