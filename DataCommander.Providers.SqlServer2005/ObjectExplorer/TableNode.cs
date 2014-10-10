namespace DataCommander.Providers.SqlServer2005
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using DataCommander.Foundation.Diagnostics;
    using DataCommander.Foundation.Text;
    using DataCommander.Foundation.Windows.Forms;
    using Microsoft.SqlServer.Management.Common;
    using Microsoft.SqlServer.Management.Smo;

    internal sealed class TableNode : ITreeNode
    {
        private static ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private DatabaseNode database;
        private string owner;
        private string name;

        public TableNode(
            DatabaseNode database,
            string owner,
            string name )
        {
            this.database = database;
            this.owner = owner;
            this.name = name;
        }

        public string Name
        {
            get
            {
                return string.Format( "{0}.{1}", this.owner, this.name );
            }
        }

        public bool IsLeaf
        {
            get
            {
                return false;
            }
        }

        IEnumerable<ITreeNode> ITreeNode.GetChildren( bool refresh )
        {
            return new ITreeNode[]
            {
                new ColumnCollectionNode(this.database, this.owner, this.name),
                new TriggerCollectionNode(this.database, this.owner, this.name)
            };
        }

        public bool Sortable
        {
            get
            {
                return false;
            }
        }

        internal static string GetSelectStatement(
            IDbConnection connection,
            FourPartName fourPartName )
        {
            Contract.Requires( connection != null );
            Contract.Requires( fourPartName != null );

            string commandText = string.Format(@"select  c.name
from    [{0}].sys.schemas s (nolock)
join    [{0}].sys.tables t (nolock)
    on s.schema_id = t.schema_id
join    [{0}].sys.columns c (nolock)
    on t.object_id = c.object_id
where
    s.name = '{1}'
    and t.name = '{2}'
order by c.column_id",
                fourPartName.Database,
                fourPartName.Owner,
                fourPartName.Name);

            var columnNames = new StringBuilder();
            bool first = true;

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using (IDataReader dataReader = connection.ExecuteReader( commandText ))
            {
                while (dataReader.Read())
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        columnNames.Append( ",\r\n        " );
                    }

                    columnNames.Append( '[' );
                    columnNames.Append( dataReader[ 0 ] );
                    columnNames.Append( ']' );
                }
            }

            string query = string.Format(
@"select  {0}
from    [{1}].[{2}].[{3}]", columnNames, fourPartName.Database, fourPartName.Owner, fourPartName.Name );

            return query;
        }

        public string Query
        {
            get
            {
                var name = new FourPartName( null, this.database.Name, this.owner, this.name );
                string connectionString = this.database.Databases.Server.ConnectionString;
                string text;
                using (var connection = new SqlConnection( connectionString ))
                {
                    connection.Open();
                    text = GetSelectStatement( connection, name );
                }
                return text;
            }
        }

        private void Open_Click( object sender, EventArgs e )
        {
            MainForm mainForm = DataCommander.Providers.Application.Instance.MainForm;
            var queryForm = (QueryForm) mainForm.ActiveMdiChild;
            string name = this.database.Name + "." + this.owner + "." + this.name;
            string query = "select * from " + name;
            queryForm.OpenTable( query );
        }

        private void Schema_Click( object sender, EventArgs e )
        {
            using (new CursorManager( Cursors.WaitCursor ))
            {
                string commandText = string.Format(
                    @"use [{0}]
exec sp_MShelpcolumns N'{1}.[{2}]', @orderby = 'id'
exec sp_MStablekeys N'{1}.[{2}]', null, 14
exec sp_MStablechecks N'{1}.[{2}]'",
                    database.Name,
                    owner,
                    name );

                log.Write( LogLevel.Trace,  commandText );
                string connectionString = this.Database.Databases.Server.ConnectionString;
                DataSet dataSet;
                using (var connection = new SqlConnection( connectionString ))
                {
                    dataSet = connection.ExecuteDataSet( commandText );
                }
                DataTable columns = dataSet.Tables[ 0 ];
                DataTable keys = dataSet.Tables[ 1 ];

                DataTable schema = new DataTable();
                schema.Columns.Add( " ", typeof( int ) );
                schema.Columns.Add( "  ", typeof( string ) );
                schema.Columns.Add( "Name", typeof( string ) );
                schema.Columns.Add( "Type", typeof( string ) );
                schema.Columns.Add( "Collation", typeof( string ) );
                schema.Columns.Add( "Formula", typeof( string ) );

                foreach (DataRow column in columns.Rows)
                {
                    string identity;

                    if (Convert.ToBoolean( column[ "col_identity" ] ))
                    {
                        identity = "IDENTITY";
                    }
                    else
                    {
                        identity = string.Empty;
                    }

                    StringBuilder sb = new StringBuilder();
                    string dbType = column[ "col_typename" ].ToString();
                    sb.Append( dbType );

                    switch (dbType)
                    {
                        case "decimal":
                        case "numeric":
                            int precision = Convert.ToInt32( column[ "col_prec" ] );
                            int scale = Convert.ToInt32( column[ "col_scale" ] );

                            if (scale == 0)
                            {
                                sb.AppendFormat( "({0})", precision );
                            }
                            else
                            {
                                sb.AppendFormat( "({0},{1})", precision, scale );
                            }

                            break;

                        case "char":
                        case "nchar":
                        case "varchar":
                        case "nvarchar":
                        case "varbinary":
                            int columnLength = (int) column[ "col_len" ];
                            string columnlengthString;

                            if (columnLength == -1)
                            {
                                columnlengthString = "max";
                            }
                            else
                            {
                                columnlengthString = columnLength.ToString();
                            }

                            sb.AppendFormat( "({0})", columnlengthString );
                            break;

                        default:
                            break;
                    }

                    if (!Convert.ToBoolean( column[ "col_null" ] ))
                    {
                        sb.Append( " not null" );
                    }

                    string collation = DataCommander.Foundation.Data.Database.GetValue<string>( column[ "collation" ], string.Empty );
                    string formula = string.Empty;

                    if (column[ "text" ] != DBNull.Value)
                    {
                        formula = column[ "text" ].ToString();
                    }

                    schema.Rows.Add( new object[]
                    {
                        column["col_id"],
                        identity,
                        column["col_name"],
                        sb.ToString(),
                        collation,
                        formula,
                    } );
                }

                if (keys.Rows.Count > 0)
                {
                    DataRow pk = (from row in keys.AsEnumerable()
                                  where row.Field<byte>( "cType" ) == 1
                                  select row).FirstOrDefault();

                    if (pk != null)
                    {
                        for (int i = 1; i <= 16; i++)
                        {
                            object keyColObj = pk[ "cKeyCol" + i ];

                            if (keyColObj == DBNull.Value)
                            {
                                break;
                            }

                            string keyCol = keyColObj.ToString();

                            string filter = string.Format( "Name = '{0}'", keyCol );
                            DataRow dataRow = schema.Select( filter )[ 0 ];
                            string identity = dataRow[ 1 ].ToString();

                            if (identity.Length > 0)
                            {
                                dataRow[ 1 ] = "PKEY," + dataRow[ 1 ];
                            }
                            else
                            {
                                dataRow[ 1 ] = "PKEY";
                            }
                        }
                    }
                }

                dataSet.Tables.Add( schema );

                MainForm mainForm = DataCommander.Providers.Application.Instance.MainForm;
                QueryForm queryForm = (QueryForm) mainForm.ActiveMdiChild;
                queryForm.ShowDataSet( dataSet );
            }
        }

        private void ScriptTable_Click( object sender, EventArgs e )
        {
            using (new CursorManager( Cursors.WaitCursor ))
            {
                var queryForm = (QueryForm) DataCommander.Providers.Application.Instance.MainForm.ActiveMdiChild;
                queryForm.SetStatusbarPanelText( "Copying table script to clipboard...", SystemColors.ControlText );
                var stopwatch = Stopwatch.StartNew();

                string connectionString = this.database.Databases.Server.ConnectionString;
                var csb = new SqlConnectionStringBuilder( connectionString );

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
                var connection = new ServerConnection( connectionInfo );
                connection.Connect();
                var server = new Server( connection );
                var database = server.Databases[ this.database.Name ];
                var table = database.Tables[ this.name, this.owner ];

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

                var stringCollection = table.Script( options );
                var sb = new StringBuilder();
                foreach (string s in stringCollection)
                {
                    sb.AppendLine( s );
                    sb.AppendLine( "GO" );
                }

                Clipboard.SetText( sb.ToString() );
                stopwatch.Stop();
                queryForm.SetStatusbarPanelText(
                    string.Format(
                        "Copying table script to clipboard finished in {0} seconds.",
                        StopwatchTimeSpan.ToString( stopwatch.ElapsedTicks, 3 ) ),
                    SystemColors.ControlText );
            }
        }

        private void Indexes_Click( object sender, EventArgs e )
        {
            string cmdText = string.Format( "use [{0}] exec sp_helpindex [{1}.{2}]", database.Name, owner, name );
            string connectionString = this.database.Databases.Server.ConnectionString;
            DataTable dataTable;
            using (var connection = new SqlConnection( connectionString ))
            {
                dataTable = connection.ExecuteDataTable( cmdText );
            }
            dataTable.TableName = string.Format( "{0} indexes", name );
            MainForm mainForm = DataCommander.Providers.Application.Instance.MainForm;
            var queryForm = (QueryForm) mainForm.ActiveMdiChild;
            var dataSet = new DataSet();
            dataSet.Tables.Add( dataTable );
            queryForm.ShowDataSet( dataSet );
        }

        private void SelectScript_Click( object sender, EventArgs e )
        {
            FourPartName name = new FourPartName( null, this.database.Name, this.owner, this.name );
            string connectionString = this.database.Databases.Server.ConnectionString;
            string selectStatement;
            using (var connection = new SqlConnection( connectionString ))
            {
                selectStatement = GetSelectStatement( connection, name );
            }
            QueryForm.ShowText( selectStatement );
        }

        private void InsertScript_Click( object sender, EventArgs e )
        {
            string commandText = string.Format(@"select
     c.name
    ,t.name as TypeName
    ,c.max_length
    ,c.precision
    ,c.scale
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
order by c.column_id", this.database.Name, this.owner, this.name);
            log.Write( LogLevel.Trace,  commandText );
            string connectionString = this.database.Databases.Server.ConnectionString;
            DataTable table;
            using (var connection = new SqlConnection( connectionString ))
            {
                table = connection.ExecuteDataTable( commandText );
            }
            var sb = new StringBuilder();

            bool first = true;
            foreach (DataRow row in table.Rows)
            {
                if (first)
                {
                    first = false;
                    sb.Append( "declare\r\n" );
                }
                else
                {
                    sb.Append( ",\r\n" );
                }

                string variableName = (string) row[ "name" ];
                variableName = char.ToLower( variableName[ 0 ] ) + variableName.Substring( 1 );
                string typeName = (string) row[ "TypeName" ];

                switch (typeName)
                {
                    case "char":
                    case "nchar":
                    case "nvarchar":
                    case "varchar":
                        short precision = row.Field<short>( "max_length" );
                        string precisionString = precision >= 0 ? precision.ToString() : "max";
                        typeName += "(" + precisionString.ToString() + ")";
                        break;

                    case "decimal":
                        int scale = row.Field<int>( "scale" );
                        if (scale == 0)
                        {
                            typeName += "(" + row[ "prec" ].ToString() + ")";
                        }
                        else
                        {
                            typeName += "(" + row[ "prec" ].ToString() + ',' + scale + ")";
                        }
                        break;

                    default:
                        break;
                }

                sb.AppendFormat( "    @{0} {1}", variableName, typeName );
            }

            sb.AppendFormat( "\r\ninsert into {0}.{1}\r\n(\r\n    ", owner, name );
            first = true;

            foreach (DataRow row in table.Rows)
            {
                if (first)
                    first = false;
                else
                    sb.Append( ',' );

                sb.Append( row[ "name" ] );
            }

            sb.Append( "\r\n)\r\nselect\r\n" );
            first = true;

            StringTable st = new StringTable( 3 );
            int last = table.Rows.Count - 1;

            for (int i = 0; i < table.Rows.Count; i++)
            {
                DataRow dataRow = table.Rows[ i ];
                StringTableRow stringTableRow = st.NewRow();
                string variableName = (string) dataRow[ "name" ];
                variableName = char.ToLower( variableName[ 0 ] ) + variableName.Substring( 1 );
                stringTableRow[ 1 ] = string.Format( "@{0}", variableName );
                string s2 = string.Format( "as {0}", dataRow[ "name" ] );

                if (i < last)
                {
                    s2 += ',';
                }

                stringTableRow[ 2 ] = s2;
                st.Rows.Add( stringTableRow );
            }

            StringWriter stringWriter = new StringWriter();
            st.Write( stringWriter, 4 );
            sb.Append( stringWriter );

            QueryForm.ShowText( sb.ToString() );
        }

        public ContextMenuStrip ContextMenu
        {
            get
            {
                ContextMenuStrip menu = new ContextMenuStrip();
                ToolStripMenuItem item = new ToolStripMenuItem( "Open", null, Open_Click );
                menu.Items.Add( item );

                item = new ToolStripMenuItem( "Script Table", null, ScriptTable_Click );
                menu.Items.Add( item );

                item = new ToolStripMenuItem( "Schema", null, Schema_Click );
                menu.Items.Add( item );

                item = new ToolStripMenuItem( "Indexes", null, Indexes_Click );
                menu.Items.Add( item );

                item = new ToolStripMenuItem( "Select script", null, SelectScript_Click );
                menu.Items.Add( item );

                item = new ToolStripMenuItem( "Insert script", null, InsertScript_Click );
                menu.Items.Add( item );

                return menu;
            }
        }

        public DatabaseNode Database
        {
            get
            {
                return database;
            }
        }
    }
}