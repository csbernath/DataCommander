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

        public static ExecuteReaderResponse<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>
            Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>(List<T1> objects1, List<T2> objects2, List<T3> objects3, List<T4> objects4,
                List<T5> objects5, List<T6> objects6, List<T7> objects7, List<T8> objects8, List<T9> objects9, List<T10> objects10, List<T11> objects11, List<T12> objects12,
                List<T13> objects13, List<T14> objects14, List<T15> objects15, List<T16> objects16, List<T17> objects17, List<T18> objects18, List<T19> objects19)
        {
            return new ExecuteReaderResponse<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>(objects1, objects2, objects3, objects4, objects5,
                objects6, objects7, objects8, objects9, objects10, objects11, objects12,
                objects13, objects14, objects15, objects16, objects17, objects18, objects19);
        }
    }
}