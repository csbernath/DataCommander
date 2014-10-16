namespace DataCommander.Providers.Wmi
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Data.Common;
    using System.Management;
    using System.Xml;
    using DataCommander.Foundation.Configuration;
    using DataCommander.Foundation.Data;

    internal sealed class WmiProvider : IProvider
    {
        string IProvider.Name
        {
            get
            {
                return "WmiProvider";
            }
        }

        public DbProviderFactory DbProviderFactory
        {
            get
            {
                return WmiProviderFactory.Instance;
            }
        }

        ConnectionBase IProvider.CreateConnection(string connectionString)
        {
            return new WmiProviderConnection(connectionString);
        }

        string[] IProvider.KeyWords
        {
            get
            {
                return null;
            }
        }

        //void IProvider.SetStandardOutput(IDbCommand command, IStandardOutput standardOutput)
        //{
        //}

        bool IProvider.IsCommandCancelable
        {
            get
            {
                return false;
            }
        }

        void IProvider.DeriveParameters(IDbCommand command)
        {
        }

        DataParameterBase IProvider.GetDataParameter(IDataParameter parameter)
        {
            return null;
        }

        DataTable IProvider.GetParameterTable(IDataParameterCollection parameters)
        {
            return null;
        }

        XmlReader IProvider.ExecuteXmlReader(IDbCommand command)
        {
            return null;
        }

        DataTable IProvider.GetSchemaTable(IDataReader dataReader)
        {
            DataTable table = null;
            DataTable schemaTable = dataReader.GetSchemaTable();

            if (schemaTable != null)
            {
                table = new DataTable("SchemaTable");
                DataColumnCollection columns = table.Columns;
                columns.Add(" ", typeof (int));
                columns.Add("Name", typeof (string));
                columns.Add("CimType", typeof (string));

                for (int i = 0; i < schemaTable.Rows.Count; i++)
                {
                    DataRow row = schemaTable.Rows[i];
                    int columnOrdinal = i + 1;
                    int columnSize = (int)row["ColumnSize"];

                    table.Rows.Add(new object[]
                    {
                        columnOrdinal,
                        row["ColumnName"],
                        row["ProviderTypeStr"]
                    });
                }
            }

            return table;
        }

        DataSet IProvider.GetTableSchema(IDbConnection connection, string tableName)
        {
            return null;
        }

        //Type IProvider.ToDataType(int providerType, int columnSize)
        //{
        //    bool isArray = (providerType & 0x1000) == 0x1000;
        //    CimType cimType = new CimType();

        //    if (isArray)
        //    {
        //        cimType = (CimType)(providerType & ~0x1000);
        //    }
        //    else
        //    {
        //        cimType = (CimType)providerType;
        //    }

        //    Type type;

        //    switch (cimType)
        //    {
        //        case CimType.String:
        //            type = isArray ? typeof (string[]) : typeof (string);
        //            break;

        //        default:
        //            type = null;
        //            break;

        //    }

        //    return type;
        //}

        IDataReaderHelper IProvider.CreateDataReaderHelper(IDataReader dataReader)
        {
            return new WmiDataReaderHelper(dataReader);
        }

        //    public void GetValues(IDataReader dataReader,object[] values)
        //    {
        //      dataReader.GetValues(values);
        //    }

        DbDataAdapter IProvider.CreateDataAdapter(string selectCommandText, IDbConnection connection)
        {
            return null;
        }

        IObjectExplorer IProvider.ObjectExplorer
        {
            get
            {
                return new WmiObjectExplorer();
            }
        }

        private static void AddClassNames(ManagementClass manClass, IList list)
        {
            string className = manClass.ClassPath.ClassName;
            list.Add(className);

            ManagementObjectCollection objects = manClass.GetSubclasses();

            foreach (ManagementClass subClass in objects)
            {
                AddClassNames(subClass, list);
            }
        }

        void IProvider.ClearCompletionCache()
        {
            ConfigurationNode node = Application.Instance.ApplicationData.CurrentType;
            const string key = "ClassNames";
            node.Attributes.Remove(key);
        }

        string IProvider.GetExceptionMessage(Exception e)
        {
            return e.ToString();
        }

        Type IProvider.GetColumnType(DataColumnSchema dataColumnSchema)
        {
            return dataColumnSchema.DataType;
        }

        string IProvider.GetColumnTypeName(IProvider sourceProvider, DataRow sourceSchemaRow, string sourceDataTypeName)
        {
            throw new NotImplementedException();
        }

        void IProvider.CreateInsertCommand(
            DataTable sourceSchemaTable,
            string[] sourceDataTypeNames,
            IDbConnection destinationconnection,
            string destinationTableName,
            out IDbCommand insertCommand,
            out Converter<object, object>[] converters)
        {
            throw new NotImplementedException();
        }

        InfoMessage[] IProvider.ToInfoMessages(Exception e)
        {
            throw new NotImplementedException();
        }

        bool IProvider.CanConvertCommandToString
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        GetCompletionResponse IProvider.GetCompletion(ConnectionBase connection, IDbTransaction transaction, string text, int position)
        {
            var response = new GetCompletionResponse();
            //      string[] array = null;
            //
            //      switch (word)
            //      {
            //        case "from":
            //          ApplicationData appData = Application.Instance.ApplicationData;
            //          Folder folder = appData.CurrentType;
            //          const string key = "ClassNames";
            //          
            //          if (folder.Properties.ContainsKey(key))
            //          {
            //            array = (string[])folder.Properties[key];
            //          }
            //          else
            //          {
            //            WmiConnection wmiConnection = (WmiConnection)connection.Wrapped;
            //            ManagementClass manClass = new ManagementClass(wmiConnection.Scope.Path);
            //            ArrayList list = new ArrayList();
            //            AddClassNames(manClass,list);
            //
            //            array = new string[list.Count];
            //            list.CopyTo(array);
            //            Array.Sort(array);
            //
            //            folder.Properties.Add("ClassNames",array);
            //            appData.Save();            
            //          }
            //          break;
            //
            //        default:
            //          break;
            //      }
            //
            //      return array;
            return response;
        }

        string IProvider.CommandToString(IDbCommand command)
        {
            throw new NotImplementedException();
        }

        string[] IProvider.GetStatements(string commandText)
        {
            return new[] {commandText};
        }
    }
}