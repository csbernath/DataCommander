# Data Commander

Data Commander is a front-end for SQL and other databases.
The program has a plugin architecture for adding arbitrary data providers.

Special features of Data Commander

Feature|Data Commander|SQL Server Management Studio v17.2
---|---|---
Audit: log commands to file|Yes|No
Auto completion: Pascal case filtering|Yes|No
Auto completion: column value filter|Yes|No
Color themes|Light, dark|Light
Create insert statements from select|Yes|No
Drag & drop: file to binary constant|Yes|No
Edit rows: generate change script|Yes|No
Object explorer: find item|Yes|No
ORM: result = handler.Handle(query), [POCO](https://en.wikipedia.org/wiki/Plain_old_CLR_object)|Yes|No
Performance: startup|Fast|Slow
Result: save as Excel|Yes|No
Result: save as HTML|Yes|No
Result: filter in memory|Yes|No
Result: sort in memory|Yes|No

## [Object-relational mapping (ORM)](https://en.wikipedia.org/wiki/Object-relational_mapping)

The program generates C# (requires .NET Standard 2.0 + Foundation assembly) source code wrapper for a SQL query. The query is similar to a CQRS Query:
- Query class (query input parameters)
- QueryResult class (query output)
- QueryHandler class (the handler which executes the query)

Download and restore the SQL Server 2016 sample database from https://github.com/microsoft/sql-server-samples

Open the database with Data Commander and execute the following command:

```SQL
/* Query
{
  "Namespace": "Foundation.NetStandard20.Test.GetCustomerInvoicesNamespace",
  "Name": "GetCustomerInvoices",
  "Parameters": [
    {
      "Name": "customerId",
      "DataType": "int",
      "IsNullable": false,
      "Value": "1"
    },
    {
      "Name": "invoiceDate",
      "DataType": "date",
      "IsNullable": false,
      "Value": "'20160101'"
    }
  ],
  "Results": [
    "Customer",
    "Invoice"
  ]
}
*/
select
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
order by i.CustomerID,i.InvoiceID
```

The program generates C# classes in one file:

```C#
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading;
using Foundation.Assertions;
using Foundation.Data;
using Foundation.Data.SqlClient;

namespace Foundation.NetStandard20.Test.GetCustomerInvoicesNamespace
{
    public sealed class GetCustomerInvoicesQuery
    {
        public readonly int CustomerId;
        public readonly DateTime InvoiceDate;
    
        public GetCustomerInvoicesQuery(int customerId, DateTime invoiceDate)
        {
            CustomerId = customerId;
            InvoiceDate = invoiceDate;
        }
    }

    public sealed class GetCustomerInvoicesQueryResult
    {
        public readonly ReadOnlyCollection<Customer> Customer;
        public readonly ReadOnlyCollection<Invoice> Invoice;
    
        public GetCustomerInvoicesQueryResult(ReadOnlyCollection<Customer> customer, ReadOnlyCollection<Invoice> invoice)
        {
            Customer = customer;
            Invoice = invoice;
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
    
    public sealed class GetCustomerInvoicesHandler
    {
        private const string CommandText = @"use WideWorldImporters

select
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
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;
    
        public GetCustomerInvoicesHandler(IDbConnection connection, IDbTransaction transaction)
        {
            Assert.IsNotNull(connection);
            _connection = connection;
            _transaction = transaction;
        }
    
        public GetCustomerInvoicesQueryResult Handle(GetCustomerInvoicesQuery query)
        {
            Assert.IsNotNull(query);
            var parameters = CreateParameters(query);
            const int commandTimeout = 0;
            var createCommandRequest = new CreateCommandRequest(CommandText, parameters, CommandType.Text, commandTimeout, _transaction);
            var executeReaderRequest = new ExecuteReaderRequest(createCommandRequest, CommandBehavior.Default, CancellationToken.None);
            return Handle(executeReaderRequest);
        }
    
        private static ReadOnlyCollection<object> CreateParameters(GetPersonQuery query)
        {
            var parameters = new SqlParameterCollectionBuilder();
            parameters.Add("customerId", query.CustomerId);
            parameters.AddDate("invoiceDate", query.InvoiceDate);
            return parameters.ToReadOnlyCollection();
        }
    
        private GetCustomerInvoicesQueryResult Handle(ExecuteReaderRequest request)
        {
            GetCustomerInvoicesQueryResult result = null;
            var executor = _connection.CreateCommandExecutor();
            executor.ExecuteReader(request, dataReader =>
            {
                var customer = dataReader.ReadResult(ReadCustomer).AsReadOnly();
                var invoice = dataReader.ReadNextResult(ReadInvoice).AsReadOnly();
                result = new GetCustomerInvoicesQueryResult(customer, invoice);
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
```

2. The generated code can be used like this:

```C#
public void TestMethod1()
{
    var csb = new SqlConnectionStringBuilder();
    csb.DataSource = @".\SQL2016_001";
    csb.InitialCatalog = "WideWorldImporters";
    csb.IntegratedSecurity = true;

    using (var connection = new SqlConnection())
    {
        connection.ConnectionString = csb.ConnectionString;
        connection.Open();

        var query = new GetCustomerInvoicesQuery(1, new DateTime(2016, 01, 01));
        var handler = new GetCustomerInvoicesHandler(connection, null);
        var result = handler.Handle(query);
    }
}

var connectionStringBuilder = new SqlConnectionStringBuilder();
connectionStringBuilder.DataSource = @".\SQL2016_001";
connectionStringBuilder.InitialCatalog = "WideWorldImporters";
connectionStringBuilder.IntegratedSecurity = true;

using (var connection = new SqlConnection(connectionStringBuilder.ConnectionString))
{
	connection.Open();
	var executor = connection.CreateCommandAsyncExecutor();

	var commandText = "waitfor delay '00:00:01'";
	var affectedRows = await executor.ExecuteNonQueryAsync(new ExecuteNonReaderRequest(commandText));

	commandText = "select top 1 i.InvoiceID from Sales.Invoices i";
	var scalar = await executor.ExecuteScalarAsync(new ExecuteNonReaderRequest(commandText));

	var commandText = @"select
c.CustomerID,
c.CustomerName
from Sales.Customers c

select
i.InvoiceID,
i.CustomerID,
i.InvoiceDate
from Sales.Invoices i
order by i.CustomerID,i.InvoiceID";
	var response = await executor.ExecuteReaderAsync(new ExecuteReaderRequest(commandText), ReadCustomer, ReadInvoice);
	var customers = response.Objects1;
	var invoices = response.Objects2;
}

```

The following plugins are already implemented:

SQL databases (client/server):

- Microsoft SQL Server
- MySQL
- Oracle
- PostgreSQL

SQL databases (file):

- Microsoft SQL Server Compact
- Microsoft Access
- Microsoft Excel 
- SQLite

Special databases:

- COM (Component Object Model)
- GAC (Global Assembly Cache)
- MSI (Windows Installer)
- TFS (Team Foundation Server)
- WMI (Windows Management Instrumentation)

Features:

- The editor has syntax highlighting, code completion for SQL statements.
- The output of a query can be displayed as data grid, text, html.
- The data grid can be exported into Excel file.

Provider.Name|Description
---|---
Msi|[Windows Installer](https://msdn.microsoft.com/en-us/library/cc185688(v=vs.85).aspx) using [Wix](http://wixtoolset.org)|
MySql|[MySQL](https://www.mysql.com/)|
Odp|Oracle using [ODP.NET provider](http://www.oracle.com/technetwork/topics/dotnet/index-085163.html) from Oracle|
OleDb|[OLE DB](https://msdn.microsoft.com/en-us/library/system.data.oledb%28v=vs.110%29.aspx)|
Oracle|Oracle using [Oracle provider](https://msdn.microsoft.com/en-us/library/system.data.oracleclient%28v=vs.110%29.aspx) from Microsoft|
PostgreSQL|[PostgreSQL](https://github.com/npgsql/Npgsql)|
SQLite|[SQLite .NET Data Provider](http://system.data.sqlite.org)|
SqlServer|[Microsoft SQL Server](http://www.microsoft.com/en-us/server-cloud/products/sql-server/) 2005 or greater|
SqlServerCe40|[Microsoft SQL Server Compact Edition 4.0](http://www.microsoft.com/en-us/download/details.aspx?id=17876)|
Tfs-15.0.0.0|[Microsoft Team Foundation Server](https://msdn.microsoft.com/en-us/vstudio/ff637362.aspx) using nuget TFS client|
Wmi|[Windows Management Instrumentation](https://msdn.microsoft.com/en-us/library/aa394582(v=vs.85).aspx)

General functions
-----------------
Main Form
---------
Connection Form
---------------

How to connect to a database

1. Click Database\Connect menu item or press Ctrl+N.
2. Click the New button.
3. Enter the connection string paramateres of the new connection.
4. Click Test button to test the connection.
5. Click OK button to save the connection string.
  
How to create a new file based database

1. Click Database\New menu item.
2. Type the file name of the new database file (SQL Server Compact, SQLite).
3. Click Save button.

How to open a file based database

1. Click Database\Open menu item or press Ctrl+O.
2. Select the required file type (Access, Excel, MSI, SQLite, SQL Server Compact)
3. Enter the file name.
4. Click Open button.

How to create a connection to a database

1.	Click the Database/Connect menu item.
2.	Click the New button.
3.	Type the name of the connection.
4.	Choose the provider (SqlServer2005, SqlServerCe40, Tfs, Wmi, etc.)
5.	Type the data source manually or click the dropdown button to enumerate the possible data sources. Click the Refresh button if you need the refresh the data source enumeration.
6.	Check the integrated security or type the user id and password.
7.	Choose the initial catalog.
8.	Click the OK button.
	
How to open a new query form to a database

1.	Click Database/Connect menu item.
2.	Double click left mouse button on the selected row or select the row and click the Connect button.

How to copy the result of a query from a source database to a target database

1. Connect to the source database. Leave the query form open.
2. Connect to the target database. Now you have two query forms.
3. Type the text of the query into the source query form.
4. Click right mouse button in the source query form. Click the ‘Create table’ menu item.
5. Navigate to the Messages tab page. Copy the create table script to the clipboard.
6. Navigate to the target query form. Paste the create table script to the target query form. Execute it.
7. Navigate to the source query form. Click right mouse button and click the ‘Copy table’ menu item.
8. Wait until the end of the query execution.
9. Navigate to the target query form. Check the content of the table.
10. Commit the transaction in the target database by clicking the Query/Commit Transaction menu item.

How to close a query form

1. Double click the top left icon of the MDI child window (Ctrl+F4 is assigned to another function).
2. The program automatically writes the content of the query textbox into the log file (%TEMP%\DataCommander.log).

Query TextBox
-------------
- Syntax highlighting
- SQL keywords are marked with blue.
- Provider specific keywords are marked with red.
- The Data Commander specific exec keyword is marked with green.

Function|Hotkey
---|---
Database/Connect to database|Ctrl+N
Database/Open file database|Ctrl+O
Database/Save all|Ctrl+Shift+S
Database/Exit|Alt+F4
Edit/Paste|Ctrl+V
Edit/Find|Ctrl+F
Edit/Find Next|F3
Edit/List Members|Ctrl+J
Edit/Goto|Ctrl+G
Query/Describe Parameters|Ctrl+P
Query/Execute Query|Ctrl+E or F5
Query/Execute Query (Single Row)|Ctrl+1
Query/Execute Query (Schema Only)|Ctrl+R
Query/Execute Query (KeyInfo)|Ctrl+K
Query/Execute Query (XML)|Ctrl+Shift+X
Query/Open Table|Ctrl+Shift+O
Query/Cancel Executing Query|Alt+Pause
Query/Parse|Ctrl+F5
Query/Result Mode/Text|Ctrl+T
Query/Result Mode/DataGrid|Ctrl+D
Query/Result Mode/ListView|Ctrl+L
Query/Close All TabPages|Ctrl+Shift+F4
Query/Create Insert Statements|Ctrl+I
Object Explorer|F8
Help/Contents|F1
Check for updates|F12

How to open a file in the query text box

1.	Drag and drop the file into the query textbox.
2.	Click Database/Save menu item or press Ctrl+S to save the file.
  	
How to go to a specified line

1.	Press Ctrl+G.
2.	Enter the line number.
3.	Press Enter.
  	
How to execute a query

1.	Type the statement text into the query textbox.
2.	Click Query/Execute Query menu item or press Ctrl+E or press F5.

How to cancel an executing query

1.	Type the statement text into the query textbox.
2.	Click Query/Execute Query menu item or press Ctrl+E or press F5.
3.	Click Query/Cancel executing query menu item or press Clrl+F12.

How to create insert into ... values(…) SQL statements from a query result

1.	Type the query statement into the query textbox.
2.	Click Query/Create insert statements menu item or press the Ctrl+I key.
3.	The generated insert statements (and the create table statement of the result schema) will be added to the result messages tab page.

How to create insert into ... select … SQL statements from a query result

1.	Type the query statement into the query textbox.
2.	Click Query/Create insert select statements menu item.
3.	The generated insert statements (and the create table statement of the result schema) will be added to the result messages tab page.

How to use the auto completion listbox

1.	Press Ctrl+J
2.	Press Up or Down key to navigate to the previous/next member of the list.
3.	Press Shift+Up or Shift+Down to navigate to the next matching member of the list.
4.	Press Ctrl+Sub to filter items.
4.	Press Enter to select the current list member or press Escape to close the listbox.

How to modify the font of the query from

1.	Click Font menu item. Consolas Regular 9pt and Courier New Regular 8 are recommended.
2.	Restart the application.

Object Explorer
---------------

How to show/hide object explorer

1.	Press F8.

How to refresh the root node of the object explorer

1.	Click Tools/Refresh Object Explorer’s root menu item.

How to find text in object explorer nodes

1.	Press Ctrl+F.

Result tab pages – Messages tab page

How to close a result set tab page

1.	Click right mouse button on the result set tab page header.
2.	Click close menu item.

Result tab pages - DataTableEditor

Top left header context menu items

1.	Copy column names
2.	Save table as
3.	Copy table
4.	Copy table as XML
5.	Edit dataview properties
6.	Unhide all columns
7.	Unhide all rows

Column header context menu items

1.	Copy column name
2.	Hide column

Row header context menu items

1.	Hide row

Cell context menu items

1.	Find
2.	Add row filter
3.	Remove row filter
4.	Copy string field
5.	Save string field as
6.	Save binary field as

How to save the table into a file

1.	Click right mouse button over the table.
2.	Click the ’Save table as’ menu item.
3.	Save the table as HTML, Fixed Width Columns or Tab Separated Values.

How to copy the table to the clipboard

1.	Click right mouse button over the table.
2.	Click the ’Copy table’ menu item.
3.	Open Microsoft Word or Excel and click Paste Special

How to sort and filter rows in a table

1.	Click right mouse button over the table.
2.	Click the ’Edit DataView Properties’ menu item. See MSDN RowFilter and Sort for more information.

How to hide/unhide columns in a table

1.	Click right mouse button over the table header.
2.	Click the Hide column or the Unhide all columns menu item.

How to hide/unhide rows in a table

1.	Select some rows.
2.	Click right mouse button over the row header.
3.	Click the Hide rows.
4.	Click right mouse button over the table header.
5.	Click hide all rows.

How to find text in a table

1.	Navigate to the data table editor to the first row and first column. The search will start from the selected cell.  If the search starts in the first row then you can find column names, too.
2.	Click Edit/Find menu item or press Ctrl+F keys.
3.	Type the text to find to the find what textbox.
4.	Click the OK button or press the Enter key.
5.	Press the F3 key to the next search result (Find Next).

How to copy a string field to the clipboard or save a string field to a file

1.	Click right mouse button over the cell which contains the string.
2.	Click right click button and click the ’Copy string field’ or the ’Save string field as’ menu item.

How to show a field as XML in Internet Explorer

1.	Type the query statement into the query textbox which results an XML value.
2.	Click Qurery/Execute Query (Xml) menu item or press Ctrl+Shift+X keys.

How to edit data in a table

1.	Type the query statement into the query textbox.
2.	Click the Query/Open Table menu item.
3.	Modify the data in the data table editor.
4.	The generated SQL script (insert, update, delete) will be added to the query textbox.

## Available providers

### Microsoft SQL Server

Object Explorer

```
Server
    Databases
        System Databases
        Database              
            Tables
		        System Tables
                Table
                    Columns
                    Triggers
                    Indexes
            Views
			    System Views
                View
                    Columns
                    Triggers
            Programmability
                Stored Procedures
                    System Stored Procedures
			    Functions
                User-defined Table Types                
                Security
                    Users
                    Roles
                    Schemas
    Security
        Logins
    Server Objects
        Linked Servers
    Jobs
```

Query textbox – code completion

How to list databases
  
1. Type use and press Crlt+J (List members)

How to list tables, views, functions in a database

1. Type select * from and press Ctrl+J

How to list columns of a table

1. Type select * from <table> <alias> where <alias>. and press Ctrl+J

How to list stored procedures, functions

1. Type exec and press Ctrl+J

How to list the distinct top 10 field values of a column

1. Type select * from <table> <alias> where <alias>.<column> = and press Ctrl+J

How to list global variables

1. Type select @@ and press Ctrl+J

How to list local variables

1. Type select @ and press Ctrl+J

### PostgreSQL

Object Explorer

	Schemas
		Sequences
		Tables
			Columns
		Views

### Team Foundation Server

Available TFS commands (and samples):

```
exec dir '<path>','recursion'
	exec dir '$/','OneLevel'

exec get 'serverPath','localPath'

exec get '$/','C:\\Download'

exec history 'path','user'
exec history '$/EzYSK'
exec history '$/','bernath'

exec status 'path','recursion','workspace','user'
exec status '$/'
exec status '$/',null,null,'bernath'

exec workspaces 'workspace','owner','computer'
exec workspaces
exec workspaces null,'bernath'
```

## Changes

- 2015-02-06: Upgrading to .NET Framework 4.5.
- 2014-10-10: Moving to Github
- 2014-05-29: [DataTableEditor] Saving data table into Excel 2007 .xlsx file.
- 2014-04-04: [CompletionForm] Adding quote handling to multipart identitifers (use [Foo.UnitTest])
- 2014-03-03: [SqlServer2005] “Adding User-Defined Table Types” node to object explorer.
- 2013-01-16: Adding “Copy table with SqlBulkCopy” context menu item in query text editor.
- 2012-12-20: [QueryForm] Adding XmlSpreadsheet result writer (Microsoft Office XML Spreadsheet 2002 format). See
http://msdn.microsoft.com/en-us/library/office/aa140066(v=office.10).aspx.
- 2012-12-18: [SqlServer2005] Adding Security/Logins node to object explorer.
- 2012-12-18: [SqlServer2005] Adding ‘System Databases’ node to object explorer.
- 2012-12-18: Adding active query form text to toolbar.
- 2012-12-18: Adding Save All menu item. Save All menu item saves all query form texts into %TEMP% directory.
- 2012-12-04: Upgrading .NET 1.0 StatusBar to NET 2.0 StatusStrip control. Colorizing errors displayed on statusbar with red.
- 2012-11-30: Adding info message severity verbose. Information severity is displayed in blue color, verbose in black.
- 2012-11-30: Handling SQL batches separated by GO (SqlServer2005)
- 2012-09-25: [SqlServer2005] Fixing collation issue in union+order by with different collations in code completion.
- 2012-07-16: Performance tuning: adding info messages to Messages textbox asynchronously.
- 2012-07-03: [SqlServer2005] Adding Column child nodes to Table node in object browser.
- 2012-06-28: Refactoring code completion for handling four part names (database.owner.name).
- 2012-05-08: Adding Tfs2010 provider.
- 2012-03-29: Adding DateTimeOffset field type handling.
- 2011-08-29: [DataTableEditor] Adding hide rows and unhide all rows menu item to row header context menu.
- 2011-08-23: [SqlStatement] The auto completion for column names now (partially) works without table alias.
- 2011-08-18: [MemberListBox] Adding Ctrl+Subtract key handler for filtering the list. Enhanced Ctrl+Up and Ctrl+Down searching.
- 2011-07-29: [DataTableEditor] Copying table as HTML copies only visible rows ordered by the data grid view’s display index.
- 2011-07-29: [DataTableEditor] Allowing column ordering by the user (data grid view’s display indexes).
- 2011-07-28: [QueryForm] Adding Parse menu item to Query menu.
- 2011-07-28: [DataTableEditor] Removing row number from row header due to performance issues.
- 2011-07-27: [DataTableEditor] Experimental. Adding row number to row header.
- 2011-07-26: [DataTableEditor] Adding ‘Copy table as XML’ menu item to context menu.
- 2011-07-14: [MainForm] Adding ‘Close All Documents’ menu item to main menu.
- 2011-07-14: [MainForm] Adding a new toolstrip panel to the main form and moving the main menu and the main toolbar to the new toolstrip panel. Removing query form toolbar (Execute, Cancel buttons) from main menu and adding it to the query form.
- 2011-07-13: Adding VisualPharm icon web site to references.
- 2011-07-13: [MainForm] Using the built in MDI child window menu item instead of custom code.
- 2011-07-13: [QueryForm] Adding dropdown toolbar button for executing query and removing the menu item from the main menu.
- 2011-07-13: Changing HTML result table font to Tahoma 8pt.
- 2011-07-12: [SqlServerCe40] Adding shrink database and compact database menu items to object explorer Tables node context menu.
- 2011-07-12: [SqlServerCe40] Adding auto completion for tables and columns.
- 2011-06-25: [DataTableEditor] Adding hide column and unhide all columns menu item to column header context menu.
- 2011-06-23: [SqlServer2005] Adding Databases node to object explorer.
- 2011-06-21: Adding a new code completion: How to list the distinct top 10 field values of a column.
- 2011-06-20: Adding DataViewProperties form .
- 2011-06-17: [QueryForm] refactoring result set handling. A new static tab page has been added to the result tab control. Every result set will be added to this static tab page as a tab control with tab pages per table.
- 2011-06-16: [MainForm] replacing AboutForm with a simple MessageBox 
- 2011-06-15: [QueryForm] adding command text logging to Messages tab page
- 2011-06-15: [SqlStatement] enhancing SQL statement parsing
-	@parameter = value
-	default value
-	unicode  strings: N’…’
- 2011-06-06: [QueryForm] Implementing case sensitive mode when finding text in non-richtextbox controls like DataTable, TreeView.
- 2011-06-03: [DataTableViewer] Improving clipboard handling (SetDataObject with multiple attempts instead of SetText)
- 2011-06-03: [QueryTextBox] Adding [ and ] characters to word separators (e.g. [varchar])
- 2011-06-03: [SqlServer2005] Adding ‘Script Table’ menu item to table node context menu in object explorer (using SMO).
- 2011-06-02: [SqlServer2005] Adding ShortStringSize attribute to app.config for SQL Server 2005 provider.
- 2011-06-02: Removing DataCommander.rtf embedded resource. Adding Help/Content menu item which opens DataCommander.docx directly.
- 2011-06-02: Automatic backup of query statements. The query form writes the text of the form into the log file when closing the form.
- 2011-06-02: [ConnectionForm] Making Connection selector window resizable.
- 2011-06-02: [SQLite] Using the new SQLite .NET provider from http://system.data.sqlite.org.
- 2011-06-02: [DataTableViewer] Speeding up DataGridView with double buffering.
- 2011-06-01: [SqlServer2005] Uprading and fixing  intellisense to new sys views
- 2011-05-30: [SqlServer2005] Moving system stored procedures node under stored procedures node in object exlorer
- 2011-03-26: Creating  Microsoft.NET 4.0 based version of Data Commander.
- 2002-01-??: Creating SqlUtil for querying Oracle and VB6 COM objects ADO recordsets via VBScript.

## License

This program is freeware and released under the [GNU General Public License](https://www.gnu.org/licenses/gpl-3.0.txt).

## Development environment

- .NET Framework 4.7
- C# 7.0
- Visual Studio Community 2017

## Credits

![JetBrains Resharper](https://github.com/csbernath/DataCommander/blob/master/resharper-logo.ico)JetBrains Resharper

## References

- [Microsoft SQL Server Management Studio v17.0](https://msdn.microsoft.com/en-us/library/hh213248.aspx)
- [pgAdmin: PostgreSQL Tools](http://www.pgadmin.org/)
- [TOAD for SQL Server](http://www.quest.com/toad-for-sql-server/)
- [PL/SQL Developer](http://www.allroundautomations.com/plsqldev.html)
- [Query Express](http://www.albahari.com/queryexpress.html)
- [DBU](http://www.sydlow.com)
- [Blueshell Data Guy](http://www.blueshell.com/EbDg.asp)
- [QueryCommander](http://sourceforge.net/projects/querycommander/)
- [Free icons](http://www.visualpharm.com/must_have_icon_set/)
