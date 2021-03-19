using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Foundation.Collections.ReadOnly;
using Foundation.Data;
using Foundation.Data.DbQueryBuilding;
using Foundation.Data.SqlClient;
using Foundation.Data.SqlClient.SqlStatementFactories;
using Foundation.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OrmSamples
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            //var records = Enumerable.Range(0, 100)
            //    .Select(i => new OrmSampleTable(Guid.NewGuid(), 0, i.ToString(), DateTime.Now))
            //    .ToList();
            //var insertSqlStatement = OrmSampleTableSqlStatementFactory.CreateInsertSqlStatement(records);
            //var updateSqlStatements = records
            //    .Select(record => new OrmSampleTable(record.Id, record.Version + 1, record.Text + ".updated", DateTime.Now))
            //    .Select(record => OrmSampleTableSqlStatementFactory.CreateUpdateSqlStatement(record, 0))
            //    .ToList();
            //var deleteSqlStatements = records
            //    .Select(record => OrmSampleTableSqlStatementFactory.CreateDeleteSqlStatement(record.Id, record.Version))
            //    .ToList();
            //var textBuilder = new TextBuilder();
            //textBuilder.Add(insertSqlStatement);
            //textBuilder.Add(updateSqlStatements.SelectMany(i => i));
            //textBuilder.Add(deleteSqlStatements.SelectMany(i => i));
            //var commandText = textBuilder.ToLines().ToIndentedString("    ");

            var updateSqlStatement = OrmSampleTableSqlStatementFactory.CreateUpdateSqlStatement(new OrmSampleTable(Guid.NewGuid(), 0, null, DateTime.Now), 0);
            var commandText = updateSqlStatement.ToIndentedString("    ");

            using (var connection = SqlConnectionFactory.Create())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var executor = connection.CreateCommandExecutor();
                    executor.ExecuteNonQuery(new CreateCommandRequest(commandText, null, CommandType.Text, null, transaction));
                    transaction.Commit();
                }
            }
        }

        public sealed class OrmSampleTable
        {
            public readonly Guid Id;
            public readonly long Version;
            public readonly string Text;
            public readonly DateTime Timestamp;

            public OrmSampleTable(Guid id, long version, string text, DateTime timestamp)
            {
                Id = id;
                Version = version;
                Text = text;
                Timestamp = timestamp;
            }
        }

        public static class OrmSampleTableSqlStatementFactory
        {
            public static ReadOnlyCollection<Line> CreateInsertSqlStatement(IEnumerable<OrmSampleTable> records)
            {
                var columns = new[]
                {
                    "Id",
                    "Version",
                    "Text",
                    "Timestamp"
                };
                var rows = records.Select(record => new[]
                {
                    record.Id.ToSqlConstant(),
                    record.Version.ToSqlConstant(),
                    record.Text.ToNullableNVarChar(),
                    record.Timestamp.ToSqlConstant()
                }).ToReadOnlyCollection();
                var insertSqlStatement = InsertSqlStatementFactory.Create("dbo.OrmSampleTable", columns, rows);
                return insertSqlStatement;
            }

            public static ReadOnlyCollection<Line> CreateUpdateSqlStatement(OrmSampleTable record, long expectedVersion)
            {
                var setColumns = new[]
                {
                    new ColumnNameValue("Version", record.Version.ToSqlConstant()),
                    new ColumnNameValue("Text", record.Text.ToNullableNVarChar()),
                    new ColumnNameValue("Timestamp", record.Timestamp.ToSqlConstant())
                };
                var whereColumns = new[]
                {
                    new ColumnNameValue("Id", record.Id.ToSqlConstant()),
                    new ColumnNameValue("Version", expectedVersion.ToSqlConstant())
                };
                var updateSqlStatement = UpdateSqlStatementFactory.Create("dbo.OrmSampleTable", setColumns, whereColumns);
                var validation = ValidationFactory.Create("update dbo.OrmSampleTable failed");
                var textBuilder = new TextBuilder();
                textBuilder.Add(updateSqlStatement);
                textBuilder.Add(validation);
                return textBuilder.ToLines();
            }

            public static ReadOnlyCollection<Line> CreateDeleteSqlStatement(Guid id, long version)
            {
                var whereColumns = new[]
                {
                    new ColumnNameValue("Id", id.ToSqlConstant()),
                    new ColumnNameValue("Version", version.ToSqlConstant())
                };
                var deleteSqlStatement = DeleteSqlStatementFactory.Create("dbo.OrmSampleTable", whereColumns);
                var validation = ValidationFactory.Create("delete dbo.OrmSampleTable failed");
                var textBuilder = new TextBuilder();
                textBuilder.Add(deleteSqlStatement);
                textBuilder.Add(validation);
                return textBuilder.ToLines();
            }
        }
    }
}