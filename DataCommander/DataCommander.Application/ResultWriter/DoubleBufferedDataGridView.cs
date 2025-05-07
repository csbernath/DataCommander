using System;
using System.Windows.Forms;

namespace DataCommander.Application.ResultWriter;

internal class DoubleBufferedDataGridView : DataGridView
{
    public DoubleBufferedDataGridView()
    {
        RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        // AccessViolationException on ToolTip that faults COMCTL32.dll - .NET 4.0
        //ShowCellToolTips = false;
        
        RowTemplate.Height = 19;
    }

    public bool PublicDoubleBuffered
    {
        get => DoubleBuffered;
        set => DoubleBuffered = value;
    }

    protected override void OnCursorChanged(EventArgs e)
    {
        base.OnCursorChanged(e);
        var bDefault = Cursor == Cursors.Default;
        DoubleBuffered = bDefault;
    }

    //protected override void OnDataBindingComplete(DataGridViewBindingCompleteEventArgs e)
    //{
    //    long ticks = Stopwatch.GetTimestamp();
    //    int rowCount = base.Rows.Count;
    //    double digitCountDouble = Math.Log10(rowCount);
    //    int digitCount = (int)Math.Ceiling(digitCountDouble);
    //    base.RowHeadersWidth = 30 + digitCount * 8;

    //    for (int i = 0;i<rowCount;i++)
    //    {
    //        base.Rows[i].HeaderCell.Value = i.ToString();
    //    }
    //    ticks = Stopwatch.GetTimestamp() - ticks;
    //    log.Write(LogLevel.Trace, true, StopwatchTimeSpan.ToString(ticks, 6));
    //}
}