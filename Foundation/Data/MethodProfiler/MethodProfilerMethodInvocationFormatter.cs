using System.Text;
using Foundation.Data.SqlClient;
using Foundation.IO;

namespace Foundation.Data.MethodProfiler;

internal sealed class MethodProfilerMethodInvocationFormatter : IFormatter
{
    void IFormatter.AppendTo(StringBuilder sb, object[] args)
    {
        var item = (MethodInvocation)args[0];
        var parent = item.Parent;
        var parentId = parent != null ? parent.Id : (int?)null;
        sb.Append(
            $"exec MethodProfilerMethodInvocation_Add @applicationId,{item.Id},{parentId.ToSqlConstant()},{item.MethodId},{item.BeginTime},{item.EndTime}\r\n");
    }
}