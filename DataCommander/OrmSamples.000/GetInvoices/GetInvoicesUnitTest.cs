using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OrmSamples.GetInvoices
{
    [TestClass]
    public class GetInvoicesUnitTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (var sqlConnection = SqlConnectionFactory.CreateSqlConnection())
            {
                sqlConnection.Open();
                var query = new GetInvoicesDbQuery();
                var queryHandler = new GetInvoicesDbQueryHandler(sqlConnection, transaction: null);
                var queryResult = queryHandler.Handle(query);
            }
        }

        [TestMethod]
        public async Task TestMethod2()
        {
            using (var sqlConnection = SqlConnectionFactory.CreateSqlConnection())
            {
                await sqlConnection.OpenAsync();
                var query = new GetInvoicesDbQuery();
                var queryHandler = new GetInvoicesDbQueryHandler(sqlConnection, transaction: null);
                var queryResult = await queryHandler.HandleAsync(query, CancellationToken.None);
            }
        }
    }
}