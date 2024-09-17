﻿using System.Text;
using Foundation.Data.SqlClient;
using Foundation.IO;

namespace Foundation.Data.MethodProfiler;

internal sealed class MethodProfilerMethodInvocationFormatter : IFormatter
{
    void IFormatter.AppendTo(StringBuilder sb, object[] args)
    {
        MethodInvocation item = (MethodInvocation)args[0];
        MethodInvocation parent = item.Parent;
        int? parentId = parent != null ? parent.Id : (int?)null;
        sb.AppendFormat("exec MethodProfilerMethodInvocation_Add @applicationId,{0},{1},{2},{3},{4}\r\n",
            item.Id,
            parentId.ToSqlConstant(),
            item.MethodId,
            item.BeginTime,
            item.EndTime);
    }
}