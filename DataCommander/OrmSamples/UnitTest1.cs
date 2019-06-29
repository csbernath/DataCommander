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

    public sealed class OrmSampleEntity
    {
        public readonly Guid Id;
        public readonly string Value;
        public readonly int Version;

        public OrmSampleEntity(Guid id, string value, int version)
        {
            Id = id;
            Value = value;
            Version = version;
        }
    }

    public static class OrmSampleEntitySqlStatementFactory
    {
        public static ReadOnlyCollection<Line> CreateInsertSqlStatement(IEnumerable<OrmSampleEntity> records)
        {
            var columns = new[]
            {
                "Id",
                "Value",
                "Version"
            };
            var rows = records.Select(record => new[]
            {
                record.Id.ToSqlConstant(),
                record.Value.ToNullableNVarChar(),
                record.Version.ToSqlConstant()
            }).ToReadOnlyCollection();
            var insertSqlStatement = InsertSqlStatementFactory.Create("dbo.OrmSampleEntity", columns, rows);
            return insertSqlStatement;
        }

        public static ReadOnlyCollection<Line> CreateSqlUpdateStatement(OrmSampleEntity record)
        {
            var identifier = new UpdateSqlStatementFactory.Column("Id", record.Id.ToSqlConstant());
            var columns = new[]
            {
                new UpdateSqlStatementFactory.Column("Value", record.Value.ToNullableNVarChar()),
                new UpdateSqlStatementFactory.Column("Version", record.Version.ToSqlConstant())
            };
            var updateSqlStatement = UpdateSqlStatementFactory.Create("dbo.OrmSampleEntity", identifier, columns);
            return updateSqlStatement;
        }

        public static ReadOnlyCollection<Line> CreateDeleteSqlStatement(int identifierValue)
        {
            var identifier = new UpdateSqlStatementFactory.Column("Id", identifierValue.ToSqlConstant());
            var deleteSqlStatement = DeleteSqlStatementFactory.Create("dbo.OrmSampleEntity", identifier);
            return deleteSqlStatement;
        }
    }
}