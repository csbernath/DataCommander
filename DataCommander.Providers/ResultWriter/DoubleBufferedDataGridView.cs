namespace DataCommander.Providers
{
    using System;
    using System.Windows.Forms;

    internal class DoubleBufferedDataGridView : DataGridView
    {
        public DoubleBufferedDataGridView()
        {
            this.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        }

        public bool PublicDoubleBuffered
        {
            get
            {
                return this.DoubleBuffered;
            }
            set
            {
                this.DoubleBuffered = value;
            }
        }

        protected override void OnCursorChanged( EventArgs e )
        {
            base.OnCursorChanged( e );
            bool bDefault = this.Cursor == Cursors.Default;
            this.DoubleBuffered = bDefault;
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
}