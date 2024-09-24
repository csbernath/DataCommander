using DataCommander.Api;
using System.Data.OleDb;

namespace DataCommander.Providers.OleDb;

internal sealed class OleDbDataReaderHelper(OleDbDataReader dataReader) : IDataReaderHelper
{
    int IDataReaderHelper.GetValues(object[] values) => dataReader.GetValues(values);
}