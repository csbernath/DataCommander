﻿using System.Data;
using Microsoft.Data.SqlClient;

namespace Foundation.Data.SqlClient;

public sealed class SqlSequence(int id)
{
    private readonly int _id = id;

    public static void CreateSchema(IDbConnection connection)
    {
        var commandText = @"create table dbo.Sequence
(
    Id int not null,
    Name varchar(128) collate Latin1_General_CI_AS not null,
    Value int not null,
    constraint PK_Sequence primary key clustered(Id)
)";

        var executor = connection.CreateCommandExecutor();
        executor.ExecuteNonQuery(new CreateCommandRequest(commandText));

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

        executor.ExecuteNonQuery(new CreateCommandRequest(commandText));
    }

    public int GetNextSequenceValue(IDbConnection connection)
    {
        var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "GetNextSequenceValue";
        var parameter = new SqlParameter("@id", SqlDbType.Int) { Value = _id };
        command.Parameters.Add(parameter);
        var scalar = command.ExecuteScalar()!;
        var value = (int)scalar;
        return value;
    }
}