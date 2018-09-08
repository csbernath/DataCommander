using System.Collections.Generic;

namespace Foundation.Data
{
    public static class ExecuteReaderResponse
    {
        public static ExecuteReaderResponse<T1, T2> Create<T1, T2>(List<T1> objects1, List<T2> objects2)
        {
            return new ExecuteReaderResponse<T1, T2>(objects1, objects2);
        }

        public static ExecuteReaderResponse<T1, T2, T3> Create<T1, T2, T3>(List<T1> objects1, List<T2> objects2, List<T3> objects3)
        {
            return new ExecuteReaderResponse<T1, T2, T3>(objects1, objects2, objects3);
        }
    }
}