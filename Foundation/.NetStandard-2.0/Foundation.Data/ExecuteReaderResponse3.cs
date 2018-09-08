using System.Collections.Generic;

namespace Foundation.Data
{
    public class ExecuteReaderResponse<T1, T2, T3>
    {
        public readonly List<T1> Result1;
        public readonly List<T2> Result2;
        public readonly List<T3> Result3;

        public ExecuteReaderResponse(List<T1> result1, List<T2> result2, List<T3> result3)
        {
            Result1 = result1;
            Result2 = result2;
            Result3 = result3;
        }
    }
}