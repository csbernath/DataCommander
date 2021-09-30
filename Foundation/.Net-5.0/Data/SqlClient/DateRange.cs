using System;

namespace Foundation.Data.SqlClient
{
    public static class DateRange
    {
        public static readonly DateTime Min = new(1, 1, 1);
        public static readonly DateTime Max = new(9999, 12, 31);
    }
}