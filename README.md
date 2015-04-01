Data Commander
==============

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

How to connect to a data source
-------------------------------
  1. Click Database\Connect menu item or press Ctrl+N.
  2. Click the New button.
  3. Enter the connection string paramateres of the new connection.
  4. Click Test button to test the connection.
  5. Click OK button to save the connection string.
  
How to create a new file based data source
------------------------------------------
  1. Click Database\New menu item.
  2. Type the file name of the new database file (SQL Server Compact, SQLite).
  3. Click Save button.

How to open a file based data source
------------------------------------
  1. Click Database\Open menu item or press Ctrl+O.
  2. Select the required file type (Access, Excel, MSI, SQLite, SQL Server Compact)
  3. Enter the file name.
  4. Click Open button.

License:
https://www.gnu.org/licenses/gpl-3.0.txt
