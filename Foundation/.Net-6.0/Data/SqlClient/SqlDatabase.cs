using System.Data;
using System.Data.SqlTypes;
using System.Globalization;
using System.Text;

namespace Foundation.Data.SqlClient
{
    /// <summary>
    /// Static helper methods for SQL Server connections.
    /// </summary>
    public static class SqlDatabase
    {
        public static readonly SqlDateTime SqlDateTimeZero = new(1900, 1, 1);

        public static string GetSysComments(IDbConnection connection, string database, string schema, string name)
        {
            var commandText = string.Format(
                CultureInfo.InvariantCulture,
                @"declare
    @schema     sysname,
    @name       sysname,    
    @schema_id  int,
    @id         int,
    @encrypted  bit

select
    @schema = {1},
    @name  = {2}

select  @schema_id = schema_id
from    [{0}].sys.schemas
where   name = @schema

select  @id    = o.id
from    [{0}].dbo.sysobjects o
where    
    o.uid       = @schema_id
    and o.name  = @name

select  @encrypted = c.encrypted
from    [{0}].dbo.syscomments c
where
    c.id        = @id
    and c.colid = 1

if @encrypted = 0
begin
    select  c.text
    from    [{0}].dbo.syscomments c
    where   c.id = @id
    order by c.colid
end
else
begin
    raiserror(15471,-1,-1,@name)    
end",
                database,
                schema.ToNullableNVarChar(),
                name.ToNullableNVarChar());

            var stringBuilder = new StringBuilder();
            var executor = connection.CreateCommandExecutor();
            executor.ExecuteReader(new ExecuteReaderRequest(commandText), dataReader =>
            {
                while (dataReader.Read())
                {
                    var s = dataReader.GetString(0);
                    stringBuilder.Append(s);
                }
            });

            return stringBuilder.ToString();
        }
    }
}