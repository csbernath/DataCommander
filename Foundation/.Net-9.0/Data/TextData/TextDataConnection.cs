using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace Foundation.Data.TextData;

/// <summary>
/// 
/// </summary>
public sealed class TextDataConnection : DbConnection
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public new TextDataCommand CreateCommand()
    {
        var command = new TextDataCommand { Connection = this };
        return command;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isolationLevel"></param>
    /// <returns></returns>
    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => throw new NotImplementedException();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="databaseName"></param>
    public override void ChangeDatabase(string databaseName) => throw new NotImplementedException();

    /// <summary>
    /// 
    /// </summary>
    public override void Close()
    {
    }

    [AllowNull]
    public override string ConnectionString
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected override DbCommand CreateDbCommand() => throw new NotImplementedException();

    /// <summary>
    /// 
    /// </summary>
    public override string DataSource => throw new NotImplementedException();

    /// <summary>
    /// 
    /// </summary>
    public override string Database => throw new NotImplementedException();

    /// <summary>
    /// 
    /// </summary>
    public override void Open()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public override string ServerVersion => throw new NotImplementedException();

    /// <summary>
    /// 
    /// </summary>
    public override ConnectionState State => ConnectionState.Open;
}