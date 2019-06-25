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

namespace OrmSamples.GetInvoices
{
    public sealed class GetInvoicesDbQuery
    {
        public GetInvoicesDbQuery()
        {
        }
    }

    public sealed class GetInvoicesDbQueryResult
    {
        public readonly ReadOnlySegmentLinkedList<Invoice> Invoices;

        public GetInvoicesDbQueryResult(ReadOnlySegmentLinkedList<Invoice> invoices)
        {
            Invoices = invoices;
        }
    }

    public sealed class Invoice
    {
        public readonly int InvoiceID;
        public readonly int CustomerID;
        public readonly int BillToCustomerID;
        public readonly int? OrderID;
        public readonly int DeliveryMethodID;
        public readonly int ContactPersonID;
        public readonly int AccountsPersonID;
        public readonly int SalespersonPersonID;
        public readonly int PackedByPersonID;
        public readonly DateTime InvoiceDate;
        public readonly string CustomerPurchaseOrderNumber;
        public readonly bool IsCreditNote;
        public readonly string CreditNoteReason;
        public readonly string Comments;
        public readonly string DeliveryInstructions;
        public readonly string InternalComments;
        public readonly int TotalDryItems;
        public readonly int TotalChillerItems;
        public readonly string DeliveryRun;
        public readonly string RunPosition;
        public readonly string ReturnedDeliveryData;
        public readonly DateTime? ConfirmedDeliveryTime;
        public readonly string ConfirmedReceivedBy;
        public readonly int LastEditedBy;
        public readonly DateTime LastEditedWhen;

        public Invoice(int invoiceID, int customerID, int billToCustomerID, int? orderID, int deliveryMethodID, int contactPersonID, int accountsPersonID, int salespersonPersonID, int packedByPersonID, DateTime invoiceDate, string customerPurchaseOrderNumber, bool isCreditNote, string creditNoteReason, string comments, string deliveryInstructions, string internalComments, int totalDryItems, int totalChillerItems, string deliveryRun, string runPosition, string returnedDeliveryData, DateTime? confirmedDeliveryTime, string confirmedReceivedBy, int lastEditedBy, DateTime lastEditedWhen)
        {
            InvoiceID = invoiceID;
            CustomerID = customerID;
            BillToCustomerID = billToCustomerID;
            OrderID = orderID;
            DeliveryMethodID = deliveryMethodID;
            ContactPersonID = contactPersonID;
            AccountsPersonID = accountsPersonID;
            SalespersonPersonID = salespersonPersonID;
            PackedByPersonID = packedByPersonID;
            InvoiceDate = invoiceDate;
            CustomerPurchaseOrderNumber = customerPurchaseOrderNumber;
            IsCreditNote = isCreditNote;
            CreditNoteReason = creditNoteReason;
            Comments = comments;
            DeliveryInstructions = deliveryInstructions;
            InternalComments = internalComments;
            TotalDryItems = totalDryItems;
            TotalChillerItems = totalChillerItems;
            DeliveryRun = deliveryRun;
            RunPosition = runPosition;
            ReturnedDeliveryData = returnedDeliveryData;
            ConfirmedDeliveryTime = confirmedDeliveryTime;
            ConfirmedReceivedBy = confirmedReceivedBy;
            LastEditedBy = lastEditedBy;
            LastEditedWhen = lastEditedWhen;
        }
    }

    public sealed class GetInvoicesDbQueryHandler
    {
        private const string CommandText = @"select  [InvoiceID],
        [CustomerID],
        [BillToCustomerID],
        [OrderID],
        [DeliveryMethodID],
        [ContactPersonID],
        [AccountsPersonID],
        [SalespersonPersonID],
        [PackedByPersonID],
        [InvoiceDate],
        [CustomerPurchaseOrderNumber],
        [IsCreditNote],
        [CreditNoteReason],
        [Comments],
        [DeliveryInstructions],
        [InternalComments],
        [TotalDryItems],
        [TotalChillerItems],
        [DeliveryRun],
        [RunPosition],
        [ReturnedDeliveryData],
        [ConfirmedDeliveryTime],
        [ConfirmedReceivedBy],
        [LastEditedBy],
        [LastEditedWhen]
from    [WideWorldImporters].[Sales].[Invoices]";
        private static int? CommandTimeout = 0;
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public GetInvoicesDbQueryHandler(IDbConnection connection, IDbTransaction transaction)
        {
            Assert.IsNotNull(connection);
            _connection = connection;
            _transaction = transaction;
        }

        public GetInvoicesDbQueryResult Handle(GetInvoicesDbQuery query)
        {
            Assert.IsNotNull(query);
            var request = ToExecuteReaderRequest(query, CancellationToken.None);
            return ExecuteReader(request);
        }

        public Task<GetInvoicesDbQueryResult> HandleAsync(GetInvoicesDbQuery query, CancellationToken cancellationToken)
        {
            Assert.IsNotNull(query);
            var request = ToExecuteReaderRequest(query, cancellationToken);
            return ExecuteReaderAsync(request);
        }

        private CreateCommandRequest ToCreateCommandRequest(GetInvoicesDbQuery query)
        {
            var parameters = ToParameters(query);
            return new CreateCommandRequest(CommandText, parameters, CommandType.Text, CommandTimeout, _transaction);
        }

        private static ReadOnlyCollection<object> ToParameters(GetInvoicesDbQuery query)
        {
            var parameters = new SqlParameterCollectionBuilder();
            return parameters.ToReadOnlyCollection();
        }

        private ExecuteReaderRequest ToExecuteReaderRequest(GetInvoicesDbQuery query, CancellationToken cancellationToken)
        {    
            var createCommandRequest = ToCreateCommandRequest(query);
            return new ExecuteReaderRequest(createCommandRequest, CommandBehavior.Default, cancellationToken);
        }

        private GetInvoicesDbQueryResult ExecuteReader(ExecuteReaderRequest request)
        {
            GetInvoicesDbQueryResult result = null;
            var executor = _connection.CreateCommandExecutor();
            executor.ExecuteReader(request, dataReader =>
            {
                var invoices = dataReader.ReadResult(128, ReadInvoice);
                result = new GetInvoicesDbQueryResult(invoices);
            });

            return result;
        }

        private async Task<GetInvoicesDbQueryResult> ExecuteReaderAsync(ExecuteReaderRequest request)
        {
            GetInvoicesDbQueryResult result = null;
            var connection = (DbConnection)_connection;
            var executor = connection.CreateCommandAsyncExecutor();
            await executor.ExecuteReaderAsync(request, async dataReader =>
            {
                var invoices = (await dataReader.ReadResultAsync(128, ReadInvoice, request.CancellationToken));
                result = new GetInvoicesDbQueryResult(invoices);
            });

            return result;
        }

        private static Invoice ReadInvoice(IDataRecord dataRecord)
        {
            var invoiceID = dataRecord.GetInt32(0);
            var customerID = dataRecord.GetInt32(1);
            var billToCustomerID = dataRecord.GetInt32(2);
            var orderID = dataRecord.GetNullableInt32(3);
            var deliveryMethodID = dataRecord.GetInt32(4);
            var contactPersonID = dataRecord.GetInt32(5);
            var accountsPersonID = dataRecord.GetInt32(6);
            var salespersonPersonID = dataRecord.GetInt32(7);
            var packedByPersonID = dataRecord.GetInt32(8);
            var invoiceDate = dataRecord.GetDateTime(9);
            var customerPurchaseOrderNumber = dataRecord.GetStringOrDefault(10);
            var isCreditNote = dataRecord.GetBoolean(11);
            var creditNoteReason = dataRecord.GetStringOrDefault(12);
            var comments = dataRecord.GetStringOrDefault(13);
            var deliveryInstructions = dataRecord.GetStringOrDefault(14);
            var internalComments = dataRecord.GetStringOrDefault(15);
            var totalDryItems = dataRecord.GetInt32(16);
            var totalChillerItems = dataRecord.GetInt32(17);
            var deliveryRun = dataRecord.GetStringOrDefault(18);
            var runPosition = dataRecord.GetStringOrDefault(19);
            var returnedDeliveryData = dataRecord.GetStringOrDefault(20);
            var confirmedDeliveryTime = dataRecord.GetNullableDateTime(21);
            var confirmedReceivedBy = dataRecord.GetStringOrDefault(22);
            var lastEditedBy = dataRecord.GetInt32(23);
            var lastEditedWhen = dataRecord.GetDateTime(24);

            return new Invoice(invoiceID, customerID, billToCustomerID, orderID, deliveryMethodID, contactPersonID, accountsPersonID, salespersonPersonID, packedByPersonID, invoiceDate, customerPurchaseOrderNumber, isCreditNote, creditNoteReason, comments, deliveryInstructions, internalComments, totalDryItems, totalChillerItems, deliveryRun, runPosition, returnedDeliveryData, confirmedDeliveryTime, confirmedReceivedBy, lastEditedBy, lastEditedWhen);
        }
    }
}