using System.Reflection;
using System.Text;
using Foundation.Data.SqlClient;
using Foundation.IO;

namespace Foundation.Data.MethodProfiler;

internal sealed class MethodFormatter : IFormatter
{
    void IFormatter.AppendTo(StringBuilder sb, object[] args)
    {
        MethodBase method = (MethodBase)args[0];
        int methodId = (int)args[1];
        sb.AppendFormat("exec MethodProfilerMethod_Add @applicationId,{0},", methodId);
        System.Type type = method.DeclaringType;
        Assembly assembly = type.Assembly;
        string assemblyName = assembly.GetName().Name;
        string typeName = type.FullName;
        string methodName = method.Name;
        sb.Append(assemblyName.ToNullableNVarChar());
        sb.Append(',');
        sb.Append(typeName.ToNullableNVarChar());
        sb.Append(',');
        sb.Append(methodName.ToNullableNVarChar());
        sb.AppendLine();
    }
}