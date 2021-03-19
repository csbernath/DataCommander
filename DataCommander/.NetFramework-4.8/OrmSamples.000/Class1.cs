//using System.Collections.ObjectModel;
//using System.Linq;
//using Foundation.Data.SqlClient;
//using Foundation.Linq;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Foundation.Text;

//namespace OrmSamples
//{
//    public class PeopleRecord
//    {
//        public readonly int PersonID;
//        public readonly string FullName;
//        public readonly string PreferredName;
//    }

//    public sealed class RecordField
//    {
//        public readonly string Name;
//        public readonly string Type;
//        public readonly bool IsNullable;
//    }

//    [TestClass]
//    public class Class1
//    {
//        [TestMethod]
//        public void F()
//        {
//            CreateInsert(new PeopleRecord());
//        }

//        public static string CreateInsert(PeopleRecord record)
//        {
//            var sqlTable = new SqlTable("Application.People", new[]
//            {
//                "PersonID", "FullName", "PreferredName"
//            }.ToReadOnlyCollection());

//            var row = new[]
//            {
//                record.PersonID.ToSqlConstant(),
//                record.FullName.ToNullableNVarChar(),
//                record.PreferredName.ToNullableNVarChar()
//            };

//            var insertSqlStatement = InsertSqlStatementFactory.Row(sqlTable.TableName, sqlTable.ColumnNames, row);

//            var commandText = insertSqlStatement.ToString("    ");

//            return null;
//        }
//    }
//}