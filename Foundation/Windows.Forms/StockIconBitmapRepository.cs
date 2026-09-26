using System.Collections.Concurrent;
using System.Drawing;

namespace Foundation.Windows.Forms;

public class StockIconBitmapRepository
{
    private static readonly ConcurrentDictionary<StockIconId, Bitmap> StockIconBitmaps = new();

    public static Bitmap GetStockIconBitmap(StockIconId stockIconId)
    {
        var contains = StockIconBitmaps.TryGetValue(stockIconId, out var stockIconBitmap);
        if (!contains)
        {
            var stockIcon = SystemIcons.GetStockIcon(stockIconId);
            stockIconBitmap = stockIcon.ToBitmap();
            var added = StockIconBitmaps.TryAdd(stockIconId, stockIconBitmap);
            if (!added)
                stockIconBitmap.Dispose();
        }

        return stockIconBitmap!;
    }
}