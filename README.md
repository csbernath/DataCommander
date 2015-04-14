Data Commander
==============

This program is freeware and released under the [GNU General Public License](https://www.gnu.org/licenses/gpl-3.0.txt).

Data Commander is a front-end for SQL databases and other data sources.
The program has a plugin architecture for adding arbitrary data providers.

The following plugins are already implemented:

Relational database servers:
  * iDB2
  * Microsoft SQL Server
  * Oracle

Relational file databases:
  * SQLite  
  * Microsoft SQL Server Compact
  * Microsoft Access
  * Microsoft Excel 
  
Special data sources:
  * COM
  * GAC
  * MSI (Windows Installer)
  * TFS (Team Foundation Server)
  * WMI
     
Written in C# and can be run on Windows, .NET Framework 4.5.
The editor has syntax highlighting, code completion for SQL statements.
The output of a query can be displayed as data grid, text, html.
The data grid can be exported into Excel.

|Provider.Name|Namespace|Description|
|-------------|---------|-----------|
|Msi|DataCommander.Providers.Msi|MSI (Windows Installer) provider (implemented in Data Commander) http://wix.codeplex.com/|
|Odp|Oracle.DataAccess.Client|Oracle .NET Provider from Oracle|
|OleDb|System.Data.OleDb|OLE DB .NET Data Provider|
|Oracle|System.Data.Oracle|Oracle .NET Data Provider from Microsoft|
|SQLite|System.Data.SQLite|SQLite .NET Data Provider from http://system.data.sqlite.org|
|SqlServer2005|System.Data.SqlClient|Microsoft SQL Server 2005 or greater|
|SqlServerCe40|System.Data.SqlServerCe|Microsoft SQL Server Compact Edition 4.0|
|Tfs2010|DataCommander.Providers.Tfs2010|Microsoft Team Foundation Server 2010 (implemented in Data Commander)|
|Wmi|DataCommander.Provider.Wmi|WMI .NET Data Provider (implemented in Data Commander)

General functions
-----------------
Main Form
---------
Connection Form
---------------

How to connect to a data source
  1. Click Database\Connect menu item or press Ctrl+N.
  2. Click the New button.
  3. Enter the connection string paramateres of the new connection.
  4. Click Test button to test the connection.
  5. Click OK button to save the connection string.
  
How to create a new file based data source
  1. Click Database\New menu item.
  2. Type the file name of the new database file (SQL Server Compact, SQLite).
  3. Click Save button.

How to open a file based data source
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
  1.	Connect to the source database. Leave the query form open.
  2.	Connect to the target database. Now you have two query forms.
  3.	Type the text of the query into the source query form.
  4.	Click right mouse button in the source query form. Click the ‘Create table’ menu item.
  5.	Navigate to the Messages tab page. Copy the create table script to the clipboard.
  6.	Navigate to the target query form. Paste the create table script to the target query form. Execute it.
  7.	Navigate to the source query form. Click right mouse button and click the ‘Copy table’ menu item.
  8.	Wait until the end of the query execution.
  9.	Navigate to the target query form. Check the content of the table.
  10.	Commit the transaction in the target database by clicking the Query/Commit Transaction menu item.

How to close a query form
  1.	Double click the top left icon of the MDI child window (Ctrl+F4 is assigned to another function).
  2.	The program automatically writes the content of the query textbox into the log file (%TEMP%\DataCommander.log).

Query TextBox
-------------
  * Syntax highlighting
  * SQL keywords are marked with blue.
  * Provider specific keywords are marked with red.
  * The Data Commander specific exec keyword is marked with green.

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
  1.	Click Ctrl+F.

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

Available providers

Microsoft SQL Server 2005
 -------------------------
Object Explorer

	Server
		Databases
			System Databases
			Database
				Tables
					System Tables
				Views
					System Views
				Programmability
					Stored Procedures
						System Stored Procedures
					Functions
					User-defined Table Types
				Security
					Users
					Roles
		Security
			Logins
		Server Objects
			Linked Servers
		Jobs

Query textbox – code completion

How to list databases
  1.	Type use and press Crlt+J (List members)

How to list tables, views, functions in a database
  1.	Type select * from and press Ctrl+J

How to list columns of a table
  1.	Type select * from <table> <alias> where <alias>. and press Ctrl+J

How to list stored procedures, functions
  1.	Type exec and press Ctrl+J

How to list the distinct top 10 field values of a column
  1.	Type select * from <table> <alias> where <alias>.<column> = and press Ctrl+J
How to list global variables
  1.	Type select @@ and press Ctrl+J
How to list local variables
  1.	Type select @ and press Ctrl+J

Team Foundation Server
Available TFS commands (and samples):
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

References
----------
  * [Microsoft SQL Server Management Studio](https://msdn.microsoft.com/en-us/library/hh213248.aspx)
  * [TOAD for SQL Server](http://www.quest.com/toad-for-sql-server/)
  * [PL/SQL Developer](http://www.allroundautomations.com/plsqldev.html)
  * [Query Express](http://www.albahari.com/queryexpress.html)
  * [DBU](http://www.sydlow.com)
  * [Blueshell Data Guy](http://www.blueshell.com/EbDg.asp)
  * [QueryCommander](http://sourceforge.net/projects/querycommander/)
  * [Free icons](http://www.visualpharm.com/must_have_icon_set/)
