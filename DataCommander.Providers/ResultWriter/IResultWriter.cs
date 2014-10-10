namespace DataCommander.Providers
{
    using System.Data;

    internal interface IResultWriter
    {
        void Begin();

        void BeforeExecuteReader(IProvider provider, IDbCommand command);

        void AfterExecuteReader();

        void AfterCloseReader(int affectedRows);

        void WriteTableBegin(DataTable schemaTable, string[] dataTypeNames);

        void FirstRowReadBegin();

        void FirstRowReadEnd();

        void WriteRows(object[][] rows, int rowCount);

        void WriteTableEnd();

        void WriteParameters(IDataParameterCollection parameters);

        void End();
    }
}