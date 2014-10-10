namespace DataCommander.Foundation.Data.SqlClient
{
    using System;

    internal interface ISqlLogItem
    {
        String CommandText
        {
            get;
        }
    }
}