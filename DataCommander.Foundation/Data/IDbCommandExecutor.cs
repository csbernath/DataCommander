namespace DataCommander.Foundation.Data
{
    using System;
    using System.Data;

    public interface IDbCommandExecutor
    {
        void Execute(Action<IDbConnection> execute);
    }
}