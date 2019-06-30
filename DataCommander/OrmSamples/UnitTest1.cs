using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            var records = Enumerable.Range(0, 100)
                .Select(i => new OrmSampleTable(Guid.NewGuid(), i.ToString(), i, DateTime.Now))
                .ToList();
            var insertSqlStatement = OrmSampleTableSqlStatementFactory.CreateInsertSqlStatement(records);
            var updateSqlStatements = records
                .Select(record =>
                {
                    var version = record.Version + 1;
                    var text = record.Text + version;
                    return new OrmSampleTable(record.Id, text, version, DateTime.Now);
                })
                .Select(record => OrmSampleTableSqlStatementFactory.CreateSqlUpdateStatement(record))
                .ToList();
            var deleteSqlStatements = records
                .Select(record => OrmSampleTableSqlStatementFactory.CreateDeleteSqlStatement(record.Id))
                .ToList();
            var textBuilder = new TextBuilder();
            textBuilder.Add(insertSqlStatement);
            textBuilder.Add(updateSqlStatements.SelectMany(i => i));
            textBuilder.Add(deleteSqlStatements.SelectMany(i => i));
            var commandText = textBuilder.ToLines().ToIndentedString("    ");

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
            public readonly string Text;
            public readonly long Version;
            public readonly DateTime Timestamp;

            public OrmSampleTable(Guid id, string text, long version, DateTime timestamp)
            {
                Id = id;
                Text = text;
                Version = version;
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

            public static ReadOnlyCollection<Line> CreateSqlUpdateStatement(OrmSampleTable record)
            {
                var identifier = new ColumnNameValue("Id", record.Id.ToSqlConstant());
                var columns = new[]
                {
                    new ColumnNameValue("Text", record.Text.ToNVarChar()),
                    new ColumnNameValue("Version", record.Version.ToSqlConstant()),
                    new ColumnNameValue("Timestamp", record.Timestamp.ToSqlConstant())
                };
                var updateSqlStatement = UpdateSqlStatementFactory.Create("dbo.OrmSampleTable", identifier, columns);
                return updateSqlStatement;
            }

            public static ReadOnlyCollection<Line> CreateDeleteSqlStatement(Guid identifierValue)
            {
                var identifier = new ColumnNameValue("Id", identifierValue.ToSqlConstant());
                var deleteSqlStatement = DeleteSqlStatementFactory.Create("dbo.OrmSampleTable", identifier);
                return deleteSqlStatement;
            }
        }
    }
}