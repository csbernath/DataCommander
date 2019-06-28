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

    public sealed class DictionaryItem
    {
        public readonly int InternalId;
        public readonly string Code;
        public readonly string DictionaryId;
        public readonly int ExternalId;
        public readonly string Name;
        public readonly DateTime? Start;
        public readonly DateTime? End;
        public readonly int? SzotarElemId;

        public DictionaryItem(int internalId, string code, string dictionaryId, int externalId, string name, DateTime? start, DateTime? end, int? szotarElemId)
        {
            InternalId = internalId;
            Code = code;
            DictionaryId = dictionaryId;
            ExternalId = externalId;
            Name = name;
            Start = start;
            End = end;
            SzotarElemId = szotarElemId;
        }
    }

    public static class Orm
    {
        public static ReadOnlyCollection<Line> CreateInsertSqlStatement(IEnumerable<DictionaryItem> source)
        {
            var columns = new[]
            {
                "InternalId",
                "Code",
                "DictionaryId",
                "ExternalId",
                "Name",
                "Start",
                "End",
                "SzotarElemId"
            };
            var rows = source.Select(item => new[]
            {
                item.InternalId.ToSqlConstant(),
                item.Code.ToNullableNVarChar(),
                item.DictionaryId.ToNVarChar(),
                item.ExternalId.ToSqlConstant(),
                item.Name.ToNullableNVarChar(),
                item.Start.ToSqlConstant(),
                item.End.ToSqlConstant(),
                item.SzotarElemId.ToSqlConstant()
            }).ToReadOnlyCollection();
            var insertSqlStatement = InsertSqlStatementFactory.Create("BUSINESSPARAMETERDATABASE.DictionaryItem", columns, rows);
            return insertSqlStatement;
        }

        public static ReadOnlyCollection<Line> CreateSqlUpdateStatement(DictionaryItem record)
        {
            var identifier = new UpdateSqlStatementFactory.Column("ExternalId", record.ExternalId.ToSqlConstant());
            var columns = new[]
            {
                new UpdateSqlStatementFactory.Column("InternalId", record.InternalId.ToSqlConstant()),
                new UpdateSqlStatementFactory.Column("Code", record.Code.ToNullableNVarChar()),
                new UpdateSqlStatementFactory.Column("DictionaryId", record.DictionaryId.ToNVarChar()),
                new UpdateSqlStatementFactory.Column("ExternalId", record.ExternalId.ToSqlConstant()),
                new UpdateSqlStatementFactory.Column("Name", record.Name.ToNullableNVarChar()),
                new UpdateSqlStatementFactory.Column("Start", record.Start.ToSqlConstant()),
                new UpdateSqlStatementFactory.Column("End", record.End.ToSqlConstant()),
                new UpdateSqlStatementFactory.Column("SzotarElemId", record.SzotarElemId.ToSqlConstant())
            };
            var updateSqlStatement = UpdateSqlStatementFactory.Create("BUSINESSPARAMETERDATABASE.DictionaryItem", identifier, columns);
            return updateSqlStatement;
        }
    }
}