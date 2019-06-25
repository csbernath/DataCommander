using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OrmSamples.GetCustomerInvoices
{
    [TestClass]
    public class GetCustomerInvoicesUnitTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (var sqlConnection = SqlConnectionFactory.CreateSqlConnection())
            {
                sqlConnection.Open();
                var query = new GetCustomerInvoicesDbQuery(customerId: 1, invoiceDate: DateTime.Today);
                var queryHandler = new GetCustomerInvoicesDbQueryHandler(sqlConnection, transaction: null);
                var queryResult = queryHandler.Handle(query);
            }
        }
    }
}