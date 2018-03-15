using System.Collections.Generic;

namespace Foundation.Data
{
    public class ExecuteReaderResponse<T1, T2, T3>
    {
        public readonly List<T1> Objects1;
        public readonly List<T2> Objects2;
        public readonly List<T3> Objects3;

        public ExecuteReaderResponse(List<T1> objects1, List<T2> objects2, List<T3> objects3)
        {
            Objects1 = objects1;
            Objects2 = objects2;
            Objects3 = objects3;
        }
    }
}