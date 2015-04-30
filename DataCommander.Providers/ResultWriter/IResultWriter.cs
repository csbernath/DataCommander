namespace DataCommander.Providers
{
    using System.Data;

    internal interface IResultWriter
    {
        void Begin();

        void BeforeExecuteReader(IProvider provider, IDbCommand command);

        void AfterExecuteReader();

        void AfterCloseReader(int affectedRows);

        void WriteTableBegin(DataTable schemaTable);

        void FirstRowReadBegin();

        void FirstRowReadEnd(string[] dataTypeNames);

        void WriteRows(object[][] rows, int rowCount);

        void WriteTableEnd();

        void WriteParameters(IDataParameterCollection parameters);

        void End();
    }
}