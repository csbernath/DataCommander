using System;
using System.Data;

namespace Foundation.Data;

public interface IDbCommandExecutor
{
    void Execute(Action<IDbConnection> execute);
}