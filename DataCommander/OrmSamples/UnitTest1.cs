using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Foundation.Collections.ReadOnly;
using Foundation.Data;
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
                var executor = connection.CreateCommandExecutor();
                executor.ExecuteNonQuery(new CreateCommandRequest(commandText));
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
                    "Text",
                    "Version",
                    "Timestamp"
                };
                var rows = records.Select(record => new[]
                {
                    record.Id.ToSqlConstant(),
                    record.Text.ToNVarChar(),
                    record.Version.ToSqlConstant(),
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

                var textBuilder = new TextBuilder();
                textBuilder.Add(updateSqlStatement);
                textBuilder.Add("if @@rowcount = 0");
                using (textBuilder.AddBlock("begin", "end"))
                {
                    textBuilder.Add("raiserror('update failed',16,1)");
                    textBuilder.Add("return");
                }

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
                return deleteSqlStatement;
            }
        }
    }
}