using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Foundation.Assertions;
using Foundation.Collections.ReadOnly;
using Foundation.Data;
using Foundation.Data.SqlClient;

namespace OrmSamples.GetCustomerInvoices
{
    public sealed class GetCustomerInvoicesDbQuery
    {
        public readonly int CustomerId;
        public readonly DateTime InvoiceDate;

        public GetCustomerInvoicesDbQuery(int customerId, DateTime invoiceDate)
        {
            CustomerId = customerId;
            InvoiceDate = invoiceDate;
        }
    }

    public sealed class GetCustomerInvoicesDbQueryResult
    {
        public readonly ReadOnlySegmentLinkedList<Customer> Customers;
        public readonly ReadOnlySegmentLinkedList<Invoice> Invoices;

        public GetCustomerInvoicesDbQueryResult(ReadOnlySegmentLinkedList<Customer> customers, ReadOnlySegmentLinkedList<Invoice> invoices)
        {
            Customers = customers;
            Invoices = invoices;
        }
    }

    public sealed class Customer
    {
        public readonly int CustomerID;
        public readonly string CustomerName;

        public Customer(int customerID, string customerName)
        {
            CustomerID = customerID;
            CustomerName = customerName;
        }
    }

    public sealed class Invoice
    {
        public readonly int InvoiceID;
        public readonly int CustomerID;
        public readonly DateTime InvoiceDate;

        public Invoice(int invoiceID, int customerID, DateTime invoiceDate)
        {
            InvoiceID = invoiceID;
            CustomerID = customerID;
            InvoiceDate = invoiceDate;
        }
    }

    public sealed class GetCustomerInvoicesDbQueryHandler
    {
        private const string CommandText = @"select
    c.CustomerID,
    c.CustomerName
from Sales.Customers c
where c.CustomerID = @customerId

select
    i.InvoiceID,
    i.CustomerID,
    i.InvoiceDate
from Sales.Invoices i
where
    i.CustomerID = @customerId
    and i.InvoiceDate >= @invoiceDate
order by i.CustomerID,i.InvoiceID";
        private static int? CommandTimeout = 0;
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public GetCustomerInvoicesDbQueryHandler(IDbConnection connection, IDbTransaction transaction)
        {
            Assert.IsNotNull(connection);
            _connection = connection;
            _transaction = transaction;
        }

        public GetCustomerInvoicesDbQueryResult Handle(GetCustomerInvoicesDbQuery query)
        {
            Assert.IsNotNull(query);
            var request = ToExecuteReaderRequest(query, CancellationToken.None);
            return ExecuteReader(request);
        }

        public Task<GetCustomerInvoicesDbQueryResult> HandleAsync(GetCustomerInvoicesDbQuery query, CancellationToken cancellationToken)
        {
            Assert.IsNotNull(query);
            var request = ToExecuteReaderRequest(query, cancellationToken);
            return ExecuteReaderAsync(request);
        }

        private CreateCommandRequest ToCreateCommandRequest(GetCustomerInvoicesDbQuery query)
        {
            var parameters = ToParameters(query);
            return new CreateCommandRequest(CommandText, parameters, CommandType.Text, CommandTimeout, _transaction);
        }

        private static ReadOnlyCollection<object> ToParameters(GetCustomerInvoicesDbQuery query)
        {
            var parameters = new SqlParameterCollectionBuilder();
            parameters.Add("customerId", query.CustomerId);
            parameters.AddDate("invoiceDate", query.InvoiceDate);
            return parameters.ToReadOnlyCollection();
        }

        private ExecuteReaderRequest ToExecuteReaderRequest(GetCustomerInvoicesDbQuery query, CancellationToken cancellationToken)
        {    
            var createCommandRequest = ToCreateCommandRequest(query);
            return new ExecuteReaderRequest(createCommandRequest, CommandBehavior.Default, cancellationToken);
        }

        private GetCustomerInvoicesDbQueryResult ExecuteReader(ExecuteReaderRequest request)
        {
            GetCustomerInvoicesDbQueryResult result = null;
            var executor = _connection.CreateCommandExecutor();
            executor.ExecuteReader(request, dataReader =>
            {
                var customers = dataReader.ReadResult(128, ReadCustomer);
                var invoices = dataReader.ReadNextResult(128, ReadInvoice);
                result = new GetCustomerInvoicesDbQueryResult(customers, invoices);
            });

            return result;
        }

        private async Task<GetCustomerInvoicesDbQueryResult> ExecuteReaderAsync(ExecuteReaderRequest request)
        {
            GetCustomerInvoicesDbQueryResult result = null;
            var connection = (DbConnection)_connection;
            var executor = connection.CreateCommandAsyncExecutor();
            await executor.ExecuteReaderAsync(request, async dataReader =>
            {
                var customers = (await dataReader.ReadResultAsync(128, ReadCustomer, request.CancellationToken));
                var invoices = (await dataReader.ReadNextResultAsync(128, ReadInvoice, request.CancellationToken));
                result = new GetCustomerInvoicesDbQueryResult(customers, invoices);
            });

            return result;
        }

        private static Customer ReadCustomer(IDataRecord dataRecord)
        {
            var customerID = dataRecord.GetInt32(0);
            var customerName = dataRecord.GetString(1);

            return new Customer(customerID, customerName);
        }

        private static Invoice ReadInvoice(IDataRecord dataRecord)
        {
            var invoiceID = dataRecord.GetInt32(0);
            var customerID = dataRecord.GetInt32(1);
            var invoiceDate = dataRecord.GetDateTime(2);

            return new Invoice(invoiceID, customerID, invoiceDate);
        }
    }
}