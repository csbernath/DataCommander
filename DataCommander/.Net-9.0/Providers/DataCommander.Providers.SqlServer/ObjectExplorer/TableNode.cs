using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Collections;
using Foundation.Collections.ReadOnly;
using Foundation.Core;
using Foundation.Data;
using Foundation.Data.SqlClient;
using Foundation.Data.SqlClient.DbQueryBuilding;
using Foundation.Linq;
using Foundation.Log;
using Foundation.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Sequence = Foundation.Core.Sequence;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class TableNode(DatabaseNode databaseNode, string? owner, string? name, int id, TemporalType type)
    : ITreeNode
{
    private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();

    public DatabaseNode DatabaseNode { get; } = databaseNode;

    public int Id => id;

    public string? Name
    {
        get
        {
            string? temporalType = type switch
            {
                TemporalType.NonTemporalTable => null,
                TemporalType.HistoryTable => "History",
                TemporalType.SystemVersionedTemporalTable => "System-Versioned",
                _ => throw new ArgumentOutOfRangeException(),
            };
            if (temporalType != null)
                temporalType = $" ({temporalType})";
            return $"{owner}.{name}{temporalType}";
        }
    }

    public bool IsLeaf => false;

    async Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        List<ITreeNode> treeNodes = [];

        if (type == TemporalType.SystemVersionedTemporalTable)
        {
            string commandText = @$"select t.name,t.object_id
from [{DatabaseNode.Name}].sys.tables t
where
    t.object_id in
    (
        select t.history_table_id
        from [{DatabaseNode.Name}].sys.tables t
        where object_id = {id}
    )";
            string? historyTableName = null;
            int historyTableId = 0;
            ExecuteReaderRequest request = new ExecuteReaderRequest(commandText);
            await Db.ExecuteReaderAsync(
                DatabaseNode.Databases.Server.CreateConnection,
                request,
                async (dataReader, _) =>
                {
                    await dataReader.ReadAsync(cancellationToken);
                    historyTableName = dataReader.GetString(0);
                    historyTableId = dataReader.GetInt32(1);
                },
                cancellationToken);
            treeNodes.Add(new TableNode(DatabaseNode, owner, historyTableName, historyTableId, TemporalType.HistoryTable));
        }

        treeNodes.AddRange([
            new ColumnCollectionNode(DatabaseNode, Id),
            new KeyCollectionNode(DatabaseNode, Id),
            new TriggerCollectionNode(DatabaseNode, Id),
            new IndexCollectionNode(DatabaseNode, Id)
        ]);

        return treeNodes;
    }

    public bool Sortable => false;

    public string Query
    {
        get
        {
            DatabaseObjectMultipartName name1 = new DatabaseObjectMultipartName(null, DatabaseNode.Name, owner, name);
            using Microsoft.Data.SqlClient.SqlConnection connection = DatabaseNode.Databases.Server.CreateConnection();
            connection.Open();
            string text = GetSelectStatement(connection, name1);
            return text;
        }
    }

    public ContextMenu? GetContextMenu()
    {
        MenuItem editRows = new MenuItem("Edit Rows", EditRows, EmptyReadOnlyCollection<MenuItem>.Value);

        ReadOnlyCollection<MenuItem> dropdownItems = new[]
        {
            new MenuItem("CREATE to clipboard", CreateTableScriptToClipboard, EmptyReadOnlyCollection<MenuItem>.Value),
            new MenuItem("SELECT to clipboard", SelectScript_Click, EmptyReadOnlyCollection<MenuItem>.Value),
            new MenuItem("INSERT to clipboard", InsertScript_Click, EmptyReadOnlyCollection<MenuItem>.Value),
            new MenuItem("UPDATE to clipboard", UpdateScript_Click, EmptyReadOnlyCollection<MenuItem>.Value),
            new MenuItem("C# ORM to clipboard", CsharpOrm_Click, EmptyReadOnlyCollection<MenuItem>.Value),
            new MenuItem("C# DTO with properties to clipboard", DataTransferObjectWithProperties_Click, EmptyReadOnlyCollection<MenuItem>.Value)
        }.ToReadOnlyCollection();
        MenuItem scriptTableAs = new MenuItem("Script Table as", null, dropdownItems);

        MenuItem schema = new MenuItem("Schema", Schema_Click, EmptyReadOnlyCollection<MenuItem>.Value);
        MenuItem indexes = new MenuItem("Indexes", Indexes_Click, EmptyReadOnlyCollection<MenuItem>.Value);

        ReadOnlyCollection<MenuItem> items = new[] { editRows, scriptTableAs, schema, indexes }.ToReadOnlyCollection();
        ContextMenu menu = new ContextMenu(items);

        return menu;
    }

    internal static string GetSelectStatement(IDbConnection connection, DatabaseObjectMultipartName databaseObjectMultipartName)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(databaseObjectMultipartName);

        string commandText = $@"select  c.name
from    [{databaseObjectMultipartName.Database}].sys.schemas s (nolock)
join    [{databaseObjectMultipartName.Database}].sys.objects o (nolock)
    on s.schema_id = o.schema_id
join    [{databaseObjectMultipartName.Database}].sys.columns c (nolock)
    on o.object_id = c.object_id
where
    s.name = '{databaseObjectMultipartName.Schema}'
    and o.name = '{databaseObjectMultipartName.Name}'
order by c.column_id";

        StringBuilder columnNames = new StringBuilder();
        bool first = true;

        if (connection.State != ConnectionState.Open)
            connection.Open();

        IDbCommandExecutor executor = connection.CreateCommandExecutor();
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

        string query =
            $@"select  {columnNames}
from    [{databaseObjectMultipartName.Database}].[{databaseObjectMultipartName.Schema}].[{
    databaseObjectMultipartName.Name}]";

        return query;
    }

    private void EditRows(object sender, EventArgs e)
    {
        string name1 = DatabaseNode.Name + "." + owner + "." + name;
        string query = "select * from " + name1;
        IQueryForm queryForm = (IQueryForm)sender;            
        queryForm.EditRows(query);
    }

    private void Schema_Click(object sender, EventArgs e)
    {
        string commandText = string.Format(
            @"use [{0}]
exec sp_MShelpcolumns N'{1}.[{2}]', @orderby = 'id'
exec sp_MStablekeys N'{1}.[{2}]', null, 14
exec sp_MStablechecks N'{1}.[{2}]'", DatabaseNode.Name, owner, name);

        Log.Write(LogLevel.Trace, commandText);
        DataSet dataSet;
        using (Microsoft.Data.SqlClient.SqlConnection connection = DatabaseNode.Databases.Server.CreateConnection())
        {
            IDbCommandExecutor executor = connection.CreateCommandExecutor();
            dataSet = executor.ExecuteDataSet(new ExecuteReaderRequest(commandText), CancellationToken.None);
        }

        DataTable columns = dataSet.Tables[0];
        DataTable keys = dataSet.Tables[1];

        DataTable schema = new DataTable();
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

            StringBuilder sb = new StringBuilder();
            string? dbType = column["col_typename"].ToString();
            sb.Append(dbType);

            switch (dbType)
            {
                case "decimal":
                case "numeric":
                    int precision = Convert.ToInt32(column["col_prec"]);
                    int scale = Convert.ToInt32(column["col_scale"]);

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
                    int columnLength = (int)column["col_len"];
                    string columnlengthString;

                    if (columnLength == -1)
                        columnlengthString = "max";
                    else
                        columnlengthString = columnLength.ToString();

                    sb.AppendFormat("({0})", columnlengthString);
                    break;
            }

            if (!Convert.ToBoolean(column["col_null"])) sb.Append(" not null");

            string collation = ValueReader.GetValue(column["collation"], string.Empty);
            string? formula = string.Empty;

            if (column["text"] != DBNull.Value) formula = column["text"].ToString();

            schema.Rows.Add(column["col_id"], identity, column["col_name"], sb.ToString(), collation, formula);
        }

        if (keys.Rows.Count > 0)
        {
            DataRow? pk = (from row in keys.AsEnumerable()
                where row.Field<byte>("cType") == 1
                select row).FirstOrDefault();

            if (pk != null)
                for (int i = 1; i <= 16; i++)
                {
                    object keyColObj = pk["cKeyCol" + i];

                    if (keyColObj == DBNull.Value) break;

                    string? keyCol = keyColObj.ToString();

                    string filter = $"Name = '{keyCol}'";
                    DataRow dataRow = schema.Select(filter)[0];
                    string? identity = dataRow[1].ToString();

                    if (identity.Length > 0)
                        dataRow[1] = "PKEY," + dataRow[1];
                    else
                        dataRow[1] = "PKEY";
                }
        }

        dataSet.Tables.Add(schema);

        IQueryForm queryForm = (IQueryForm)sender;
        queryForm.ShowDataSet(dataSet);
    }

    private void CreateTableScriptToClipboard(object? sender, EventArgs e)
    {
        IQueryForm? queryForm = (IQueryForm)sender;
        queryForm.SetStatusbarPanelText("Copying table script to clipboard...");
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        ICancelableOperationForm cancelableOperationForm = queryForm.CreateCancelableOperationForm(cancellationTokenSource, TimeSpan.FromSeconds(1),
            "Copying table script to clipboard...", string.Empty);
        Stopwatch stopwatch = Stopwatch.StartNew();
        string text = cancelableOperationForm.Execute(new Task<string>(GetCreateTableScript));
        long elapsedTicks = stopwatch.ElapsedTicks;
        queryForm.SetClipboardText(text);
        queryForm.SetStatusbarPanelText($"Copying CREATE TABLE script to clipboard finished in {StopwatchTimeSpan.ToString(elapsedTicks, 3)} seconds.");
    }

    private string GetCreateTableScript()
    {
        SqlConnectionInfo connectionInfo = SqlObjectScripter.CreateSqlConnectionInfo(DatabaseNode.Databases.Server.ConnectionStringAndCredential);
        ServerConnection connection = new ServerConnection(connectionInfo);
        connection.Connect();
        Server server = new Server(connection);
        Database database = server.Databases[DatabaseNode.Name];
        Table table = database.Tables[name, owner];

        ScriptingOptions options = new ScriptingOptions
        {
            AnsiPadding = true,
            Default = true,
            DriAll = true,
            ExtendedProperties = true,
            IncludeDatabaseContext = false,
            Indexes = true,
            Permissions = true,
            SchemaQualify = true,
            SchemaQualifyForeignKeysReferences = true,
            ScriptBatchTerminator = true
        };

        System.Collections.Specialized.StringCollection stringCollection = table.Script(options);
        StringBuilder stringBuilder = new StringBuilder();
        bool first = true;
        foreach (string? stringCollectionItem in stringCollection)
        {
            if (first)
                first = false;
            else
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("GO");
            }

            string reindented = stringCollectionItem.Replace("\t", "    ");
            stringBuilder.Append(reindented);
        }

        return stringBuilder.ToString();
    }

    private void Indexes_Click(object sender, EventArgs e)
    {
        string commandText = $"use [{DatabaseNode.Name}] exec sp_helpindex [{owner}.{name}]";
        DataTable dataTable;
        using (Microsoft.Data.SqlClient.SqlConnection connection = DatabaseNode.Databases.Server.CreateConnection())
        {
            IDbCommandExecutor executor = connection.CreateCommandExecutor();
            dataTable = executor.ExecuteDataTable(new ExecuteReaderRequest(commandText), CancellationToken.None);
        }

        dataTable.TableName = $"{name} indexes";

        DataSet dataSet = new DataSet();
        dataSet.Tables.Add(dataTable);

        IQueryForm queryForm = (IQueryForm)sender;            
        queryForm.ShowDataSet(dataSet);
    }

    private void SelectScript_Click(object? sender, EventArgs e)
    {
        DatabaseObjectMultipartName name1 = new DatabaseObjectMultipartName(null, DatabaseNode.Name, owner, name);
        string selectStatement;
        using (Microsoft.Data.SqlClient.SqlConnection connection = DatabaseNode.Databases.Server.CreateConnection())
        {
            selectStatement = GetSelectStatement(connection, name1);
        }

        IQueryForm? queryForm = (IQueryForm)sender;
        queryForm.SetClipboardText(selectStatement);
        queryForm.SetStatusbarPanelText("Copying script to clipboard finished.");
    }

    private sealed class Column(string columnName, string typeName, short maxLength, byte precision, byte scale, bool? isNullable)
    {
        public readonly string ColumnName = columnName;
        public readonly string TypeName = typeName;
        public readonly short MaxLength = maxLength;
        public readonly byte Precision = precision;
        public readonly byte Scale = scale;
        public readonly bool? IsNullable = isNullable;
    }

    private static Column ReadColumn(IDataRecord dataRecord)
    {
        string columnName = dataRecord.GetStringOrDefault(0);
        string typeName = dataRecord.GetString(1);
        short maxLength = dataRecord.GetInt16(2);
        byte precision = dataRecord.GetByte(3);
        byte scale = dataRecord.GetByte(4);
        bool? isNullable = dataRecord.GetNullableBoolean(5);

        return new Column(columnName, typeName, maxLength, precision, scale, isNullable);
    }

    private void InsertScript_Click(object? sender, EventArgs e)
    {
        string commandText = string.Format(@"select
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
order by c.column_id", DatabaseNode.Name, owner, name);
        Log.Write(LogLevel.Trace, commandText);
        ReadOnlySegmentLinkedList<Column> columns = Db.ExecuteReader(
            DatabaseNode.Databases.Server.CreateConnection,
            new ExecuteReaderRequest(commandText),
            128,
            ReadColumn);

        StringBuilder stringBuilder = new StringBuilder();
        bool first = true;
        foreach (Column? column in columns)
        {
            if (first)
                first = false;
            else
                stringBuilder.Append("\r\n");

            stringBuilder.Append("declare");

            string variableName = column.ColumnName;
            variableName = char.ToLower(variableName[0]) + variableName[1..];
            string typeName = column.TypeName;

            switch (typeName)
            {
                case SqlDataTypeName.Char:
                case SqlDataTypeName.VarChar:
                {
                        short maxLength = column.MaxLength;
                        string maxLengthString = maxLength >= 0 ? maxLength.ToString() : "max";
                    typeName += "(" + maxLengthString + ")";
                }
                    break;

                case SqlDataTypeName.NChar:
                case SqlDataTypeName.NVarChar:
                {
                        short maxLength = column.MaxLength;
                        string maxLengthString = maxLength >= 0 ? (maxLength / 2).ToString() : "max";
                    typeName += "(" + maxLengthString + ")";
                }
                    break;

                case SqlDataTypeName.Decimal:
                    byte precision = column.Precision;
                    byte scale = column.Scale;
                    if (scale == 0)
                        typeName += "(" + precision + ")";
                    else
                        typeName += "(" + precision + ',' + scale + ")";
                    break;
            }

            stringBuilder.Append($" @{variableName} {typeName}");

            if (column.IsNullable == false)
                stringBuilder.Append(" /*not null*/");
        }

        stringBuilder.AppendFormat("\r\n\r\ninsert into {0}.{1}\r\n(\r\n    ", owner, name);
        first = true;

        foreach (Column? column in columns)
        {
            if (first)
                first = false;
            else
                stringBuilder.Append(',');

            stringBuilder.Append(column.ColumnName);
        }

        stringBuilder.Append("\r\n)\r\nselect\r\n");
        first = true;

        StringTable stringTable = new StringTable(3);
        Sequence sequence = new Sequence();
        foreach (Column? column in columns)
        {
            StringTableRow stringTableRow = stringTable.NewRow();
            string variableName = column.ColumnName;
            variableName = char.ToLower(variableName[0]) + variableName[1..];
            stringTableRow[1] = $"@{variableName}";

            string text = $"as {column.ColumnName}";
            if (sequence.Next() < columns.Count - 1)
                text += ',';

            stringTableRow[2] = text;
            stringTable.Rows.Add(stringTableRow);
        }

        stringBuilder.Append(stringTable.ToString(4));

        IQueryForm? queryForm = (IQueryForm)sender;
        queryForm.SetClipboardText(stringBuilder.ToString());            
        queryForm.SetStatusbarPanelText("Copying script to clipboard finished.");
    }

    private string CreateUpdateScript()
    {
        GetTableSchemaResult getTableSchemaResult;
        using (Microsoft.Data.SqlClient.SqlConnection connection = DatabaseNode.Databases.Server.CreateConnection())
        {
            connection.Open();
            string tableName = $"{DatabaseNode.Name}.{owner}.{name}";
            getTableSchemaResult = TableSchema.GetTableSchema(connection, tableName);
        }

        TextBuilder textBuilder = new TextBuilder();

        textBuilder.Add($"update {Name}");
        textBuilder.Add("set");
        using (textBuilder.Indent(1))
        {
            int last = getTableSchemaResult.Columns.Count - 1;
            foreach (IndexedItem<Api.Column> item in getTableSchemaResult.Columns.SelectIndexed())
            {
                Api.Column column = item.Value;
                StringBuilder line = new StringBuilder();
                line.Append($"{column.ColumnName} = @{column.ColumnName}");
                if (item.Index < last)
                    line.Append(',');
                textBuilder.Add(line.ToString());
            }
        }

        if (getTableSchemaResult.UniqueIndexColumns.Count > 0)
        {
            textBuilder.Add("where");
            using (textBuilder.Indent(1))
            {
                int last = getTableSchemaResult.UniqueIndexColumns.Count - 1;
                foreach (IndexedItem<UniqueIndexColumn> item in getTableSchemaResult.UniqueIndexColumns.SelectIndexed())
                {
                    int columnId = item.Value.ColumnId;
                    Api.Column column = getTableSchemaResult.Columns.First(i => i.ColumnId == columnId);
                    StringBuilder line = new StringBuilder();
                    line.Append($"{column.ColumnName} = @{column.ColumnName}");
                    if (item.Index < last)
                        line.Append(',');
                    textBuilder.Add(line.ToString());
                }
            }
        }

        string script = textBuilder.ToLines().ToIndentedString("    ");
        return script;
    }

    private void UpdateScript_Click(object? sender, EventArgs e)
    {
        string script = CreateUpdateScript();

        IQueryForm? queryForm = (IQueryForm)sender;
        queryForm.SetClipboardText(script);

        queryForm.SetStatusbarPanelText("Copying script to clipboard finished.");
    }

    private void CsharpOrm_Click(object? sender, EventArgs e)
    {
        GetTableSchemaResult getTableSchemaResult;
        using (Microsoft.Data.SqlClient.SqlConnection connection = DatabaseNode.Databases.Server.CreateConnection())
        {
            connection.Open();

            string databaseName = DatabaseNode.Name.Contains('.')
                ? $"[{DatabaseNode.Name}]"
                : DatabaseNode.Name;

            string tableName = $"{databaseName}.{owner}.{name}";
            getTableSchemaResult = TableSchema.GetTableSchema(connection, tableName);
        }

        ReadOnlyCollection<DataTransferObjectField> dataTransferObjectFields = getTableSchemaResult.Columns
            .Select(column =>
            {
                string name = column.ColumnName;
                string typeName = column.TypeName;
                bool? isNullable = column.IsNullable;
                string csharpTypeName = SqlDataTypeRepository.SqlDataTypes.First(i => i.SqlDataTypeName == typeName).CSharpTypeName;
                CSharpType csharpType = CSharpTypeArray.CSharpTypes.First(i => i.Name == csharpTypeName);
                if (isNullable == true && csharpType.Type.IsValueType)
                    csharpTypeName += "?";

                return new DataTransferObjectField(name, csharpTypeName);
            })
            .ToReadOnlyCollection();
        string dataTransferObject = DataTransferObjectFactory.CreateDataTransferObject(name, dataTransferObjectFields).ToIndentedString("    ");

        ReadOnlyCollection<Foundation.Data.SqlClient.DbQueryBuilding.Column> columns = getTableSchemaResult.Columns
            .Select(i => new Foundation.Data.SqlClient.DbQueryBuilding.Column(i.ColumnName, i.TypeName, i.IsNullable == true))
            .ToReadOnlyCollection();
        ReadOnlyCollection<Line> createInsertSqlSqlStatementMethod = CreateInsertSqlStatementMethodFactory.Create(owner, name, columns);

        Foundation.Data.SqlClient.DbQueryBuilding.Column? identifierColumn = getTableSchemaResult.UniqueIndexColumns
            .Select(i => getTableSchemaResult.Columns.First(j => j.ColumnId == i.ColumnId))
            .Select(i => new Foundation.Data.SqlClient.DbQueryBuilding.Column(i.ColumnName, i.TypeName, i.IsNullable == true))
            .FirstOrDefault();
        Foundation.Data.SqlClient.DbQueryBuilding.Column? versionColumn = columns.FirstOrDefault(i => i.ColumnName == "Version");

        ReadOnlyCollection<Line>? createUpdateSqlStatementMethod;            
        ReadOnlyCollection<Line>? createDeleteSqlStatementMethod;

        if (identifierColumn != null)
        {
            columns = columns
                .Where(i => i.ColumnName != identifierColumn.ColumnName)
                .ToReadOnlyCollection();

            createUpdateSqlStatementMethod = CreateUpdateSqlStatementMethodFactory.Create(owner, name, identifierColumn, versionColumn, columns);

            createDeleteSqlStatementMethod = CreateDeleteSqlStatementMethodFactory.Create(owner, name, identifierColumn, versionColumn);
        }
        else
        {
            createUpdateSqlStatementMethod = null;
            createDeleteSqlStatementMethod = null;
        }

        TextBuilder textBuilder = new TextBuilder();
        textBuilder.Add(dataTransferObject);
        textBuilder.Add(Line.Empty);
        textBuilder.Add($"public static class {name}SqlStatementFactory");
        using (textBuilder.AddCSharpBlock())
        {
            textBuilder.Add(createInsertSqlSqlStatementMethod);

            if (createUpdateSqlStatementMethod != null)
            {
                textBuilder.Add(Line.Empty);
                textBuilder.Add(createUpdateSqlStatementMethod);
            }

            if (createDeleteSqlStatementMethod != null)
            {
                textBuilder.Add(Line.Empty);
                textBuilder.Add(createDeleteSqlStatementMethod);
            }
        }

        IQueryForm? queryForm = (IQueryForm)sender;
        queryForm.SetClipboardText(textBuilder.ToLines().ToIndentedString("    "));
        queryForm.SetStatusbarPanelText("Copying script to clipboard finished.");
    }

    private void DataTransferObjectWithProperties_Click(object? sender, EventArgs e)
    {
        GetTableSchemaResult getTableSchemaResult;
        using (Microsoft.Data.SqlClient.SqlConnection connection = DatabaseNode.Databases.Server.CreateConnection())
        {
            connection.Open();

            string databaseName = DatabaseNode.Name.Contains('.')
                ? $"[{DatabaseNode.Name}]"
                : DatabaseNode.Name;

            string tableName = $"{databaseName}.{owner}.{name}";
            getTableSchemaResult = TableSchema.GetTableSchema(connection, tableName);
        }

        ReadOnlyCollection<DataTransferObjectField> dataTransferObjectFields = getTableSchemaResult.Columns
            .Select(column =>
            {
                string name = column.ColumnName;
                string typeName = column.TypeName;
                bool? isNullable = column.IsNullable;
                string csharpTypeName = SqlDataTypeRepository.SqlDataTypes.First(i => i.SqlDataTypeName == typeName).CSharpTypeName;
                CSharpType csharpType = CSharpTypeArray.CSharpTypes.First(i => i.Name == csharpTypeName);
                if (isNullable == true && csharpType.Type.IsValueType)
                    csharpTypeName += "?";

                return new DataTransferObjectField(name, csharpTypeName);
            })
            .ToReadOnlyCollection();

        string classWithProperties = DataTransferObjectWithPropertiesFactory.Create(name, dataTransferObjectFields).ToIndentedString("    ");

        IQueryForm? queryForm = (IQueryForm)sender;
        queryForm.SetClipboardText(classWithProperties);
        queryForm.SetStatusbarPanelText("Copying script to clipboard finished.");
    }
}