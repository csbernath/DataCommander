namespace DataCommander.Foundation.Data
{
    using System.Collections.Generic;

    public static class ExecuteReaderResponse
    {
        public static ExecuteReaderResponse<TRow1, TRow2> Create<TRow1, TRow2>(List<TRow1> rows1, List<TRow2> rows2)
        {
            return new ExecuteReaderResponse<TRow1, TRow2>(rows1, rows2);
        }

        public static ExecuteReaderResponse<TRow1, TRow2, TRow3> Create<TRow1, TRow2, TRow3>(List<TRow1> rows1, List<TRow2> rows2, List<TRow3> rows3)
        {
            return new ExecuteReaderResponse<TRow1, TRow2, TRow3>(rows1, rows2, rows3);
        }
    }
}