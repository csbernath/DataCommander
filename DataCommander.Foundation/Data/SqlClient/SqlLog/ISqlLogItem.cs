namespace DataCommander.Foundation.Data.SqlClient
{
    using System;

    internal interface ISqlLogItem
    {
        string CommandText
        {
            get;
        }
    }
}