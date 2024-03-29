﻿using System.Data;
using DataCommander.Api;

namespace DataCommander.Application;

internal interface IResultWriter
{
    void Begin(IProvider provider);
    void BeforeExecuteReader(AsyncDataAdapterCommand asyncDataAdapterCommand);
    void AfterExecuteReader();
    void AfterCloseReader(int affectedRows);
    void WriteTableBegin(DataTable schemaTable);
    void FirstRowReadBegin();
    void FirstRowReadEnd(string[] dataTypeNames);
    void WriteRows(object[][] rows, int rowCount);
    void WriteTableEnd();
    void WriteParameters(IDataParameterCollection parameters);
    void End();
}