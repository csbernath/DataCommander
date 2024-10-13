using System.Reflection;
using System.Text;
using Foundation.Data.SqlClient;
using Foundation.IO;

namespace Foundation.Data.MethodProfiler;

internal sealed class MethodFormatter : IFormatter
{
    void IFormatter.AppendTo(StringBuilder stringBuilder, object[] args)
    {
        var method = (MethodBase)args[0];
        var methodId = (int)args[1];
        stringBuilder.AppendFormat("exec MethodProfilerMethod_Add @applicationId,{0},", methodId);
        var type = method.DeclaringType!;
        var assembly = type.Assembly;
        var assemblyName = assembly.GetName().Name!;
        var typeName = type.FullName!;
        var methodName = method.Name;
        stringBuilder.Append(assemblyName.ToNullableNVarChar());
        stringBuilder.Append(',');
        stringBuilder.Append(typeName.ToNullableNVarChar());
        stringBuilder.Append(',');
        stringBuilder.Append(methodName.ToNullableNVarChar());
        stringBuilder.AppendLine();
    }
}