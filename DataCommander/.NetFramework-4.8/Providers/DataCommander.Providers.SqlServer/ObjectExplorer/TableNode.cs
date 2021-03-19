using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DataCommander.Providers.Query;
using DataCommander.Providers2;
using Foundation.Assertions;
using Foundation.Collections;
using Foundation.Collections.ReadOnly;
using Foundation.Core;
using Foundation.Data;
using Foundation.Data.DbQueryBuilding;
using Foundation.Data.SqlClient;
using Foundation.Linq;
using Foundation.Log;
using Foundation.Text;
using Foundation.Windows.Forms;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Sequence = Foundation.Core.Sequence;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    internal sealed class TableNode : ITreeNode
    {
        private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
        private readonly string _name;
        private readonly string _owner;

        public TableNode(DatabaseNode databaseNode, string owner, string name, int id)
        {
            DatabaseNode = databaseNode;
            _owner = owner;
            _name = name;
            Id = id;
        }

        public DatabaseNode DatabaseNode { get; }
        public int Id { get; }
        public string Name => $"{_owner}.{_name}";
        public bool IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            return new ITreeNode[]
            {
                new ColumnCollectionNode(DatabaseNode, Id),
                new TriggerCollectionNode(DatabaseNode, Id),
                new IndexCollectionNode(DatabaseNode, Id)
            };
        }

        public bool Sortable => false;

        public string Query
        {
            get
            {
                var name = new DatabaseObjectMultipartName(null, DatabaseNode.Name, _owner, _name);
                var connectionString = DatabaseNode.Databases.Server.ConnectionString;
                string text;
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    text = GetSelectStatement(connection, name);
                }

                return text;
            }
        }

        public ContextMenuStrip ContextMenu
        {
            get
            {
                var menu = new ContextMenuStrip();
                var item = new ToolStripMenuItem("Edit Rows", null, EditRows);
                menu.Items.Add(item);

                var scriptTableAs = new ToolStripMenuItem("Script Table as");
                scriptTableAs.DropDownItems.Add(new ToolStripMenuItem("CREATE to clipboard", null, ScriptTable_Click));
                scriptTableAs.DropDownItems.Add(new ToolStripMenuItem("SELECT to clipboard", null, SelectScript_Click));
                scriptTableAs.DropDownItems.Add(new ToolStripMenuItem("INSERT to clipboard", null, InsertScript_Click));
                scriptTableAs.DropDownItems.Add(new ToolStripMenuItem("C# ORM to clipboard", null, CsharpOrm_Click));
                menu.Items.Add(scriptTableAs);

                item = new ToolStripMenuItem("Schema", null, Schema_Click);
                menu.Items.Add(item);

                item = new ToolStripMenuItem("Indexes", null, Indexes_Click);
                menu.Items.Add(item);

                return menu;
            }
        }

        internal static string GetSelectStatement(IDbConnection connection, DatabaseObjectMultipartName databaseObjectMultipartName)
        {
            Assert.IsNotNull(connection);
            Assert.IsNotNull(databaseObjectMultipartName);

            var commandText = $@"select  c.name
from    [{databaseObjectMultipartName.Database}].sys.schemas s (nolock)
join    [{databaseObjectMultipartName.Database}].sys.objects o (nolock)
    on s.schema_id = o.schema_id
join    [{databaseObjectMultipartName.Database}].sys.columns c (nolock)
    on o.object_id = c.object_id
where
    s.name = '{databaseObjectMultipartName.Schema}'
    and o.name = '{databaseObjectMultipartName.Name}'
order by c.column_id";

            var columnNames = new StringBuilder();
            var first = true;

            if (connection.State != ConnectionState.Open)
                connection.Open();

            var executor = connection.CreateCommandExecutor();
            executor.ExecuteReader(new ExecuteReaderRequest(commandText), dataReader =>
            {
                while (dataReader.Read())
                {
                    if (first)
                        first = false;
                    else
                        columnNames.Append(",\r\n        ");

                    columnNames.Append('[');
                    columnNames.Append(dataReader[0]);
                    columnNames.Append(']');
                }
            });

            var query =
                $@"select  {columnNames}
from    [{databaseObjectMultipartName.Database}].[{databaseObjectMultipartName.Schema}].[{
                    databaseObjectMultipartName.Name}]";

            return query;
        }

        private void EditRows(object sender, EventArgs e)
        {
            var mainForm = DataCommanderApplication.Instance.MainForm;
            var queryForm = (QueryForm) mainForm.ActiveMdiChild;
            var name = DatabaseNode.Name + "." + _owner + "." + _name;
            var query = "select * from " + name;
            queryForm.EditRows(query);
        }

        private void Schema_Click(object sender, EventArgs e)
        {
            using (new CursorManager(Cursors.WaitCursor))
            {
                var commandText = string.Format(
                    @"use [{0}]
exec sp_MShelpcolumns N'{1}.[{2}]', @orderby = 'id'
exec sp_MStablekeys N'{1}.[{2}]', null, 14
exec sp_MStablechecks N'{1}.[{2}]'", DatabaseNode.Name, _owner, _name);

                Log.Write(LogLevel.Trace, commandText);
                var connectionString = DatabaseNode.Databases.Server.ConnectionString;
                DataSet dataSet;
                using (var connection = new SqlConnection(connectionString))
                {
                    var executor = connection.CreateCommandExecutor();
                    dataSet = executor.ExecuteDataSet(new ExecuteReaderRequest(commandText));
                }

                var columns = dataSet.Tables[0];
                var keys = dataSet.Tables[1];

                var schema = new DataTable();
                schema.Columns.Add(" ", typeof(int));
                schema.Columns.Add("  ", typeof(string));
                schema.Columns.Add("Name", typeof(string));
                schema.Columns.Add("Type", typeof(string));
                schema.Columns.Add("Collation", typeof(string));
                schema.Columns.Add("Formula", typeof(string));

                foreach (DataRow column in columns.Rows)
                {
                    string identity;

                    if (Convert.ToBoolean(column["col_identity"]))
                        identity = "IDENTITY";
                    else
                        identity = string.Empty;

                    var sb = new StringBuilder();
                    var dbType = column["col_typename"].ToString();
                    sb.Append(dbType);

                    switch (dbType)
                    {
                        case "decimal":
                        case "numeric":
                            var precision = Convert.ToInt32(column["col_prec"]);
                            var scale = Convert.ToInt32(column["col_scale"]);

                            if (scale == 0)
                                sb.AppendFormat("({0})", precision);
                            else
                                sb.AppendFormat("({0},{1})", precision, scale);

                            break;

                        case "char":
                        case "nchar":
                        case "varchar":
                        case "nvarchar":
                        case "varbinary":
                            var columnLength = (int) column["col_len"];
                            string columnlengthString;

                            if (columnLength == -1)
                                columnlengthString = "max";
                            else
                                columnlengthString = columnLength.ToString();

                            sb.AppendFormat("({0})", columnlengthString);
                            break;
                    }

                    if (!Convert.ToBoolean(column["col_null"])) sb.Append(" not null");

                    var collation = ValueReader.GetValue(column["collation"], string.Empty);
                    var formula = string.Empty;

                    if (column["text"] != DBNull.Value) formula = column["text"].ToString();

                    schema.Rows.Add(column["col_id"], identity, column["col_name"], sb.ToString(), collation, formula);
                }

                if (keys.Rows.Count > 0)
                {
                    var pk = (from row in keys.AsEnumerable()
                        where row.Field<byte>("cType") == 1
                        select row).FirstOrDefault();

                    if (pk != null)
                        for (var i = 1; i <= 16; i++)
                        {
                            var keyColObj = pk["cKeyCol" + i];

                            if (keyColObj == DBNull.Value) break;

                            var keyCol = keyColObj.ToString();

                            var filter = $"Name = '{keyCol}'";
                            var dataRow = schema.Select(filter)[0];
                            var identity = dataRow[1].ToString();

                            if (identity.Length > 0)
                                dataRow[1] = "PKEY," + dataRow[1];
                            else
                                dataRow[1] = "PKEY";
                        }
                }

                dataSet.Tables.Add(schema);

                var mainForm = DataCommanderApplication.Instance.MainForm;
                var queryForm = (QueryForm) mainForm.ActiveMdiChild;
                queryForm.ShowDataSet(dataSet);
            }
        }

        private void ScriptTable_Click(object sender, EventArgs e)
        {
            using (new CursorManager(Cursors.WaitCursor))
            {
                var queryForm = (QueryForm) DataCommanderApplication.Instance.MainForm.ActiveMdiChild;
                queryForm.SetStatusbarPanelText("Copying table script to clipboard...",
                    queryForm.ColorTheme != null ? queryForm.ColorTheme.ForeColor : SystemColors.ControlText);
                var stopwatch = Stopwatch.StartNew();

                var connectionString = DatabaseNode.Databases.Server.ConnectionString;
                var csb = new SqlConnectionStringBuilder(connectionString);

                var connectionInfo = new SqlConnectionInfo();
                connectionInfo.ApplicationName = csb.ApplicationName;
                connectionInfo.ConnectionTimeout = csb.ConnectTimeout;
                connectionInfo.DatabaseName = csb.InitialCatalog;
                connectionInfo.EncryptConnection = csb.Encrypt;
                connectionInfo.MaxPoolSize = csb.MaxPoolSize;
                connectionInfo.MinPoolSize = csb.MinPoolSize;
                connectionInfo.PacketSize = csb.PacketSize;
                connectionInfo.Pooled = csb.Pooling;
                connectionInfo.ServerName = csb.DataSource;
                connectionInfo.UseIntegratedSecurity = csb.IntegratedSecurity;
                connectionInfo.WorkstationId = csb.WorkstationID;
                if (!csb.IntegratedSecurity)
                {
                    connectionInfo.UserName = csb.UserID;
                    connectionInfo.Password = csb.Password;
                }

                var connection = new ServerConnection(connectionInfo);
                connection.Connect();
                var server = new Server(connection);
                var database = server.Databases[DatabaseNode.Name];
                var table = database.Tables[_name, _owner];

                var options = new ScriptingOptions();
                options.Indexes = true;
                options.Permissions = true;
                options.IncludeDatabaseContext = false;
                options.Default = true;
                options.AnsiPadding = true;
                options.DriAll = true;
                options.ExtendedProperties = true;
                options.ScriptBatchTerminator = true;
                options.SchemaQualify = true;
                options.SchemaQualifyForeignKeysReferences = true;

                var stringCollection = table.Script(options);
                var sb = new StringBuilder();
                foreach (var s in stringCollection)
                {
                    sb.AppendLine(s);
                    sb.AppendLine("GO");
                }

                Clipboard.SetText(sb.ToString());
                stopwatch.Stop();
                queryForm.SetStatusbarPanelText(
                    $"Copying table script to clipboard finished in {StopwatchTimeSpan.ToString(stopwatch.ElapsedTicks, 3)} seconds.",
                    queryForm.ColorTheme != null ? queryForm.ColorTheme.ForeColor : SystemColors.ControlText);
            }
        }

        private void Indexes_Click(object sender, EventArgs e)
        {
            var commandText = $"use [{DatabaseNode.Name}] exec sp_helpindex [{_owner}.{_name}]";
            var connectionString = DatabaseNode.Databases.Server.ConnectionString;
            DataTable dataTable;
            using (var connection = new SqlConnection(connectionString))
            {
                var executor = connection.CreateCommandExecutor();
                dataTable = executor.ExecuteDataTable(new ExecuteReaderRequest(commandText));
            }

            dataTable.TableName = $"{_name} indexes";
            var mainForm = DataCommanderApplication.Instance.MainForm;
            var queryForm = (QueryForm) mainForm.ActiveMdiChild;
            var dataSet = new DataSet();
            dataSet.Tables.Add(dataTable);
            queryForm.ShowDataSet(dataSet);
        }

        private void SelectScript_Click(object sender, EventArgs e)
        {
            var name = new DatabaseObjectMultipartName(null, DatabaseNode.Name, _owner, _name);
            var connectionString = DatabaseNode.Databases.Server.ConnectionString;
            string selectStatement;
            using (var connection = new SqlConnection(connectionString))
            {
                selectStatement = GetSelectStatement(connection, name);
            }

            Clipboard.SetText(selectStatement);
            var queryForm = (QueryForm) DataCommanderApplication.Instance.MainForm.ActiveMdiChild;
            queryForm.SetStatusbarPanelText("Copying script to clipboard finished.",
                queryForm.ColorTheme != null ? queryForm.ColorTheme.ForeColor : SystemColors.ControlText);
        }

        private sealed class Column
        {
            public readonly string ColumnName;
            public readonly string TypeName;
            public readonly short MaxLength;
            public readonly byte Precision;
            public readonly byte Scale;
            public readonly bool? IsNullable;

            public Column(string columnName, string typeName, short maxLength, byte precision, byte scale, bool? isNullable)
            {
                ColumnName = columnName;
                TypeName = typeName;
                MaxLength = maxLength;
                Precision = precision;
                Scale = scale;
                IsNullable = isNullable;
            }
        }

        private static Column ReadColumn(IDataRecord dataRecord)
        {
            var columnName = dataRecord.GetStringOrDefault(0);
            var typeName = dataRecord.GetString(1);
            var maxLength = dataRecord.GetInt16(2);
            var precision = dataRecord.GetByte(3);
            var scale = dataRecord.GetByte(4);
            var isNullable = dataRecord.GetNullableBoolean(5);

            return new Column(columnName, typeName, maxLength, precision, scale, isNullable);
        }

        private void InsertScript_Click(object sender, EventArgs e)
        {
            var commandText = string.Format(@"select
    c.name,
    t.name as TypeName,
    c.max_length,
    c.precision,
    c.scale,
    c.is_nullable
from [{0}].sys.schemas s (nolock)
join [{0}].sys.objects o (nolock)
	on s.schema_id = o.schema_id
join [{0}].sys.columns c (nolock)
	on o.object_id = c.object_id
join [{0}].sys.types t (nolock)
	on c.user_type_id = t.user_type_id
where
	s.name = '{1}'
	and o.name = '{2}'
order by c.column_id", DatabaseNode.Name, _owner, _name);
            Log.Write(LogLevel.Trace, commandText);
            var connectionString = DatabaseNode.Databases.Server.ConnectionString;
            var columns = SqlClientFactory.Instance.ExecuteReader(connectionString, new ExecuteReaderRequest(commandText), 128, ReadColumn);

            var stringBuilder = new StringBuilder();
            var first = true;
            foreach (var column in columns)
            {
                if (first)
                {
                    first = false;
                    stringBuilder.Append("declare\r\n");
                }
                else
                    stringBuilder.Append(",\r\n");

                var variableName = column.ColumnName;
                variableName = char.ToLower(variableName[0]) + variableName.Substring(1);
                var typeName = column.TypeName;

                switch (typeName)
                {
                    case SqlDataTypeName.Char:
                    case SqlDataTypeName.NChar:
                    case SqlDataTypeName.NVarChar:
                    case SqlDataTypeName.VarChar:
                        var maxLength = column.MaxLength;
                        var precisionString = maxLength >= 0 ? maxLength.ToString() : "max";
                        typeName += "(" + precisionString + ")";
                        break;

                    case SqlDataTypeName.Decimal:
                        var precision = column.Precision;
                        var scale = column.Scale;
                        if (scale == 0)
                            typeName += "(" + precision + ")";
                        else
                            typeName += "(" + precision + ',' + scale + ")";
                        break;
                }

                stringBuilder.Append($"    @{variableName} {typeName}");
            }

            stringBuilder.AppendFormat("\r\n\r\ninsert into {0}.{1}\r\n(\r\n    ", _owner, _name);
            first = true;

            foreach (var column in columns)
            {
                if (first)
                    first = false;
                else
                    stringBuilder.Append(',');

                stringBuilder.Append(column.ColumnName);
            }

            stringBuilder.Append("\r\n)\r\nselect\r\n");
            first = true;

            var stringTable = new StringTable(3);
            var sequence = new Sequence();
            foreach (var column in columns)
            {
                var stringTableRow = stringTable.NewRow();
                var variableName = column.ColumnName;
                variableName = char.ToLower(variableName[0]) + variableName.Substring(1);
                stringTableRow[1] = $"@{variableName}";

                var text = $"as {column.ColumnName}";
                if (sequence.Next() < columns.Count - 1)
                    text += ',';

                stringTableRow[2] = text;
                stringTable.Rows.Add(stringTableRow);
            }

            stringBuilder.Append(stringTable.ToString(4));

            Clipboard.SetText(stringBuilder.ToString());
            var queryForm = (QueryForm) DataCommanderApplication.Instance.MainForm.ActiveMdiChild;

            queryForm.SetStatusbarPanelText("Copying script to clipboard finished.",
                queryForm.ColorTheme != null ? queryForm.ColorTheme.ForeColor : SystemColors.ControlText);
        }

        private void CsharpOrm_Click(object sender, EventArgs e)
        {
            var connectionString = DatabaseNode.Databases.Server.ConnectionString;
            GetTableSchemaResult getTableSchemaResult;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var tableName = $"{DatabaseNode.Name}.{_owner}.{_name}";
                getTableSchemaResult = TableSchema.GetTableSchema(connection, tableName);
            }

            var dataTransferObjectFields = getTableSchemaResult.Columns
                .Select(column =>
                {
                    var name = column.ColumnName;
                    var typeName = column.TypeName;
                    var isNullable = column.IsNullable;
                    var csharpTypeName = SqlDataTypeArray.SqlDataTypes.First(i => i.SqlDataTypeName == typeName).CSharpTypeName;
                    var csharpType = CSharpTypeArray.CSharpTypes.First(i => i.Name == csharpTypeName);
                    if (isNullable == true && csharpType.Type.IsValueType)
                        csharpTypeName += "?";

                    return new DataTransferObjectField(name, csharpTypeName);
                })
                .ToReadOnlyCollection();
            var dataTransferObject = DataTransferObjectFactory.CreateDataTransferObject(_name, dataTransferObjectFields).ToIndentedString("    ");

            var columns = getTableSchemaResult.Columns
                .Select(i => new Foundation.Data.DbQueryBuilding.Column(i.ColumnName, i.TypeName, i.IsNullable == true))
                .ToReadOnlyCollection();
            var createInsertSqlSqlStatementMethod = CreateInsertSqlStatementMethodFactory.Create(_owner, _name, columns);

            var identifierColumn = getTableSchemaResult.UniqueIndexColumns
                .Select(i => getTableSchemaResult.Columns.First(j => j.ColumnId == i.ColumnId))
                .Select(i => new Foundation.Data.DbQueryBuilding.Column(i.ColumnName, i.TypeName, i.IsNullable == true))
                .First();
            var versionColumn = columns.FirstOrDefault(i => i.ColumnName == "Version");
            columns = columns.Where(i => i.ColumnName != identifierColumn.ColumnName).ToReadOnlyCollection();
            var createUpdateSqlStatementMethod = CreateUpdateSqlStatementMethodFactory.Create(_owner, _name, identifierColumn, versionColumn, columns);
            var createDeleteSqlStatementMethod = CreateDeleteSqlStatementMethodFactory.Create(_owner, _name, identifierColumn, versionColumn);

            var textBuilder = new TextBuilder();
            textBuilder.Add(dataTransferObject);
            textBuilder.Add(Line.Empty);
            textBuilder.Add($"public static class {_name}SqlStatementFactory");
            using (textBuilder.AddCSharpBlock())
            {
                textBuilder.Add(createInsertSqlSqlStatementMethod);
                textBuilder.Add(Line.Empty);
                textBuilder.Add(createUpdateSqlStatementMethod);
                textBuilder.Add(Line.Empty);
                textBuilder.Add(createDeleteSqlStatementMethod);
            }

            Clipboard.SetText(textBuilder.ToLines().ToIndentedString("    "));
            var queryForm = (QueryForm)DataCommanderApplication.Instance.MainForm.ActiveMdiChild;

            queryForm.SetStatusbarPanelText("Copying script to clipboard finished.",
                queryForm.ColorTheme != null ? queryForm.ColorTheme.ForeColor : SystemColors.ControlText);
        }
    }
}