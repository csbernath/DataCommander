﻿using System.Collections.Generic;
using System.Data;

namespace DataCommander.Providers
{
    public interface IObjectExplorer
    {
        bool Sortable { get; }
        void SetConnection(string connectionString, IDbConnection connection);
        IEnumerable<ITreeNode> GetChildren(bool refresh);
    }
}