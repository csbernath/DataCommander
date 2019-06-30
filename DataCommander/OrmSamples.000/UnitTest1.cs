using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Foundation.Collections.ReadOnly;
using Foundation.Data.SqlClient;
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
            var identifier = new UpdateSqlStatementFactory.Column("Id", record.Id.ToSqlConstant());
            var columns = new[]
            {
            new UpdateSqlStatementFactory.Column("Text", record.Text.ToNVarChar()),
            new UpdateSqlStatementFactory.Column("Version", record.Version.ToSqlConstant()),
            new UpdateSqlStatementFactory.Column("Timestamp", record.Timestamp.ToSqlConstant())
        };
            var updateSqlStatement = UpdateSqlStatementFactory.Create("dbo.OrmSampleTable", identifier, columns);
            return updateSqlStatement;
        }

        public static ReadOnlyCollection<Line> CreateDeleteSqlStatement(ToSqlConstant identifierValue)
        {
            var identifier = new UpdateSqlStatementFactory.Column("Id", identifierValue);
            var deleteSqlStatement = DeleteSqlStatementFactory.Create("dbo.OrmSampleTable", identifier);
            return deleteSqlStatement;
        }
    }
}