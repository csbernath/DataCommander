namespace DataCommander.Foundation.Data
{
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public static class ExecuteReaderResponse
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rows"></param>
        /// <typeparam name="TRow"></typeparam>
        /// <returns></returns>
        public static ExecuteReaderResponse<TRow> Create<TRow>(List<TRow> rows)
        {
            return new ExecuteReaderResponse<TRow>(rows);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rows1"></param>
        /// <param name="rows2"></param>
        /// <typeparam name="TRow1"></typeparam>
        /// <typeparam name="TRow2"></typeparam>
        /// <returns></returns>
        public static ExecuteReaderResponse<TRow1, TRow2> Create<TRow1, TRow2>(List<TRow1> rows1, List<TRow2> rows2)
        {
            return new ExecuteReaderResponse<TRow1, TRow2>(rows1, rows2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rows1"></param>
        /// <param name="rows2"></param>
        /// <param name="rows3"></param>
        /// <typeparam name="TRow1"></typeparam>
        /// <typeparam name="TRow2"></typeparam>
        /// <typeparam name="TRow3"></typeparam>
        /// <returns></returns>
        public static ExecuteReaderResponse<TRow1, TRow2, TRow3> Create<TRow1, TRow2, TRow3>(List<TRow1> rows1, List<TRow2> rows2, List<TRow3> rows3)
        {
            return new ExecuteReaderResponse<TRow1, TRow2, TRow3>(rows1, rows2, rows3);
        }
    }
}