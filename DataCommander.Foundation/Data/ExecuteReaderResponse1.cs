namespace DataCommander.Foundation.Data
{
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TRow"></typeparam>
    public class ExecuteReaderResponse<TRow>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rows"></param>
        public ExecuteReaderResponse(List<TRow> rows)
        {
            Rows = rows;
        }

        /// <summary>
        /// 
        /// </summary>
        public readonly List<TRow> Rows;

    }
}