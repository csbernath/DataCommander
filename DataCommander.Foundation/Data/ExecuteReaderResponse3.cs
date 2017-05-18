namespace DataCommander.Foundation.Data
{
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TRow1"></typeparam>
    /// <typeparam name="TRow2"></typeparam>
    /// <typeparam name="TRow3"></typeparam>
    public class ExecuteReaderResponse<TRow1, TRow2, TRow3>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rows1"></param>
        /// <param name="rows2"></param>
        /// <param name="rows3"></param>
        public ExecuteReaderResponse(List<TRow1> rows1, List<TRow2> rows2, List<TRow3> rows3)
        {
            Rows1 = rows1;
            Rows2 = rows2;
            Rows3 = rows3;
        }

        /// <summary>
        /// 
        /// </summary>
        public readonly List<TRow1> Rows1;

        /// <summary>
        /// 
        /// </summary>
        public readonly List<TRow2> Rows2;

        /// <summary>
        /// 
        /// </summary>
        public readonly List<TRow3> Rows3;
    }
}