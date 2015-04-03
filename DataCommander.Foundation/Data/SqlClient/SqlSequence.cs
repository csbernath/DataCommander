namespace DataCommander.Foundation.Data
{
    using System.Data;
    using System.Data.SqlClient;

    /// <summary>
    /// 
    /// </summary>
    public sealed class SqlSequence
    {
        private readonly int id;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public SqlSequence( int id )
        {
            this.id = id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        public static void CreateSchema( IDbConnection connection )
        {
            string commandText = @"create table dbo.Sequence
(
    Id int not null,
    Name varchar(128) collate Latin1_General_CI_AS not null,
    Value int not null,
    constraint PK_Sequence primary key clustered(Id)
)";

            connection.ExecuteNonQuery( null, commandText, CommandType.Text, 0 );

            commandText = @"create proc dbo.IncrementSequence
(
    @name varchar(128),
    @increment int = 1,
    @value int out
)
as
declare @id int
select  @id = s.Id
from    dbo.Sequence s (nolock)
where   s.Name = @name

if @@rowcount = 1
begin
    begin tran

    select  @value  = s.[Value]
    from    Sequence s with(rowlock,xlock)
    where   s.Id    = @id

    update  Sequence
    set     Value   = @value + @increment
    where   Id      = @id

    commit

    select @value as Value
end
else
begin
    raiserror('Sequence not found.',16,1)
end";

            connection.ExecuteNonQuery( null, commandText, CommandType.Text, 0 );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public int GetNextSequenceValue( IDbConnection connection )
        {
            IDbCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "GetNextSequenceValue";
            SqlParameter parameter = new SqlParameter( "@id", SqlDbType.Int ) { Value = this.id };
            command.Parameters.Add( parameter );
            object scalar = command.ExecuteScalar();
            int value = (int) scalar;
            return value;
        }
    }
}