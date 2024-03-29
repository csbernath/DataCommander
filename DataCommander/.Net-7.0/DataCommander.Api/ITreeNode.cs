﻿using System.Collections.Generic;

namespace DataCommander.Api;

public interface ITreeNode
{
    string Name { get; }
    bool IsLeaf { get; }
    IEnumerable<ITreeNode> GetChildren(bool refresh);
    bool Sortable { get; }
    string Query { get; }
    ContextMenu? GetContextMenu();
}