using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
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
            var rows = new[]
            {
                new Colors(1, "name1", 1, DateTime.Now, DateTime.Now.AddDays(1)),
                new Colors(2, "name2", 1, DateTime.Now, DateTime.Now.AddDays(1)),
                new Colors(3, "name3", 1, DateTime.Now, DateTime.Now.AddDays(1))
            };

            var lines = ColorsInsertSqlStatementFactory.CreateInsertSqlStatement(rows);
            var commandText = lines.ToIndentedString("    ");
        }
    }

    public sealed class Colors
    {
        public readonly int ColorID;
        public readonly string ColorName;
        public readonly int LastEditedBy;
        public readonly DateTime ValidFrom;
        public readonly DateTime ValidTo;

        public Colors(int colorID, string colorName, int lastEditedBy, DateTime validFrom, DateTime validTo)
        {
            ColorID = colorID;
            ColorName = colorName;
            LastEditedBy = lastEditedBy;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }
    }

    public static class ColorsInsertSqlStatementFactory
    {
        public static ReadOnlyCollection<Line> CreateInsertSqlStatement(IEnumerable<Colors> source)
        {
            var columns = new[]
            {
                "ColorID",
                "ColorName",
                "LastEditedBy",
                "ValidFrom",
                "ValidTo"
            };
            var rows = source.Select(item => new[]
            {
                item.ColorID.ToSqlConstant(),
                item.ColorName.ToNVarChar(),
                item.LastEditedBy.ToSqlConstant(),
                item.ValidFrom.ToSqlConstant(),
                item.ValidTo.ToSqlConstant()
            }).ToReadOnlyCollection();
            var insertSqlStatement = InsertSqlStatementFactory.CreateInsertSqlStatement("Warehouse", "Colors", columns, rows);
            return insertSqlStatement;
        }
    }
}