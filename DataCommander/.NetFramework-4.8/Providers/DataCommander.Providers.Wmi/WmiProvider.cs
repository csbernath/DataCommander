﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Management;
using System.Xml;
using DataCommander.Providers.Connection;
using DataCommander.Providers2;
using DataCommander.Providers2.Connection;
using Foundation.Data;

namespace DataCommander.Providers.Wmi
{
    internal sealed class WmiProvider : IProvider
    {
        string IProvider.Name => "WmiProvider";

        public DbProviderFactory DbProviderFactory => WmiProviderFactory.Instance;

        ConnectionBase IProvider.CreateConnection(string connectionString)
        {
            return new WmiProviderConnection(connectionString);
        }

        string[] IProvider.KeyWords => null;
        bool IProvider.IsCommandCancelable => false;

        void IProvider.DeriveParameters(IDbCommand command)
        {
        }

        DataParameterBase IProvider.GetDataParameter(IDataParameter parameter) => null;
        DataTable IProvider.GetParameterTable(IDataParameterCollection parameters) => null;

        DataTable IProvider.GetSchemaTable(IDataReader dataReader)
        {
            DataTable table = null;
            var schemaTable = dataReader.GetSchemaTable();

            if (schemaTable != null)
            {
                table = new DataTable("SchemaTable");
                var columns = table.Columns;
                columns.Add(" ", typeof(int));
                columns.Add("Name", typeof(string));
                columns.Add("CimType", typeof(string));

                for (var i = 0; i < schemaTable.Rows.Count; i++)
                {
                    var row = schemaTable.Rows[i];
                    var columnOrdinal = i + 1;
                    var columnSize = (int) row["ColumnSize"];

                    table.Rows.Add(new[]
                    {
                        columnOrdinal,
                        row["ColumnName"],
                        row["ProviderTypeStr"]
                    });
                }
            }

            return table;
        }

        GetTableSchemaResult IProvider.GetTableSchema(IDbConnection connection, string tableName) => throw new NotImplementedException();
        IDataReaderHelper IProvider.CreateDataReaderHelper(IDataReader dataReader) => new WmiDataReaderHelper(dataReader);
        DbDataAdapter IProvider.CreateDataAdapter(string selectCommandText, IDbConnection connection) => null;
        public IObjectExplorer CreateObjectExplorer() => new WmiObjectExplorer();

        private static void AddClassNames(ManagementClass manClass, IList list)
        {
            var className = manClass.ClassPath.ClassName;
            list.Add(className);

            var objects = manClass.GetSubclasses();

            foreach (ManagementClass subClass in objects)
                AddClassNames(subClass, list);
        }

        void IProvider.ClearCompletionCache()
        {
            var node = DataCommanderApplication.Instance.ApplicationData.CurrentType;
            const string key = "ClassNames";
            node.Attributes.Remove(key);
        }

        string IProvider.GetExceptionMessage(Exception e) => e.ToString();
        Type IProvider.GetColumnType(FoundationDbColumn dataColumnSchema) => dataColumnSchema.DataType;
        string IProvider.GetColumnTypeName(IProvider sourceProvider, DataRow sourceSchemaRow, string sourceDataTypeName) => throw new NotImplementedException();

        void IProvider.CreateInsertCommand(
            DataTable sourceSchemaTable,
            string[] sourceDataTypeNames,
            IDbConnection destinationconnection,
            string destinationTableName,
            out IDbCommand insertCommand,
            out Converter<object, object>[] converters) =>
            throw new NotImplementedException();

        List<InfoMessage> IProvider.ToInfoMessages(Exception exception) => throw new NotImplementedException();
        bool IProvider.CanConvertCommandToString => throw new NotImplementedException();

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

        string IProvider.CommandToString(IDbCommand command) => throw new NotImplementedException();

        List<Statement> IProvider.GetStatements(string commandText)
        {
            return new List<Statement>
            {
                new Statement
                {
                    LineIndex = 0,
                    CommandText = commandText
                }
            };
        }

        IDbConnectionStringBuilder IProvider.CreateConnectionStringBuilder() => new ConnectionStringBuilder();
    }
}