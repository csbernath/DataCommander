using Foundation.Data.SqlEngine;
using Microsoft.Data.SqlClient;
using Xunit;

namespace Foundation.Data.Test;

public class TestClass1
{
    [Fact]
    public async Task Test1()
    {
        var cancellationToken = CancellationToken.None;
        var commandText = @"select * from Production.Product
select * from Production.TransactionHistory";
        var executeReaderRequest = new ExecuteReaderRequest(commandText);
        List<Table>? tables = null;
        await Db.ExecuteReaderAsync(CreateSqlConnection, executeReaderRequest,
            async (dbDataReader, _) => { tables = await dbDataReader.ReadTables(cancellationToken); },
            cancellationToken);

        var product = tables[0];
        var productHistory = tables[1];
        var productKeySelector = new KeySelector(new RowSelector(new[]
        {
            new ColumnValueSelector(product.Columns["ProductID"])
        }));
        var productHistoryKeySelector = new KeySelector(new RowSelector(new[]
        {
            new ColumnValueSelector(productHistory.Columns["ProductID"])
        }));
        var keyComparer = new KeyEqualityComparer();
        var productRowSelector = new RowSelector(new IValueSelector[]
        {
            new ColumnValueSelector(product.Columns["ProductID"]),
            new ColumnValueSelector(product.Columns[2])
        });
        var productHistoryRowSelector = new RowSelector(new IValueSelector[]
        {
            new ColumnValueSelector(productHistory.Columns[0]),
            new ColumnValueSelector(productHistory.Columns[1]),
            new ColumnValueSelector(productHistory.Columns[2])
        });
        var groupJoinResultSelector =
            new GroupJoinResultSelector("JoinedTable", productRowSelector, productHistoryRowSelector);
        var joinedTable = product.GroupJoin(
            productHistory,
            productKeySelector.Select,
            productHistoryKeySelector.Select,
            groupJoinResultSelector,
            keyComparer);
    }

    private SqlConnection CreateSqlConnection()
    {
        var sqlConnectionStringBuilder = new SqlConnectionStringBuilder();
        sqlConnectionStringBuilder.DataSource = ".";
        sqlConnectionStringBuilder.InitialCatalog = "AdventureWorks2022";
        sqlConnectionStringBuilder.IntegratedSecurity = true;
        sqlConnectionStringBuilder.TrustServerCertificate = true;
        return new SqlConnection(sqlConnectionStringBuilder.ConnectionString);
    }
}