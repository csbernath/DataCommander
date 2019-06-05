using System.Reflection;
using System.Text;
using Foundation.Data.SqlClient;
using Foundation.IO;

namespace Foundation.Data.MethodProfiler
{
    internal sealed class MethodFormatter : IFormatter
    {
        void IFormatter.AppendTo(StringBuilder sb, object[] args)
        {
            var method = (MethodBase) args[0];
            var methodId = (int) args[1];
            sb.AppendFormat("exec MethodProfilerMethod_Add @applicationId,{0},", methodId);
            var type = method.DeclaringType;
            var assembly = type.Assembly;
            var assemblyName = assembly.GetName().Name;
            var typeName = type.FullName;
            var methodName = method.Name;
            sb.Append(assemblyName.ToNullableNVarChar());
            sb.Append(',');
            sb.Append(typeName.ToNullableNVarChar());
            sb.Append(',');
            sb.Append(methodName.ToNullableNVarChar());
            sb.AppendLine();
        }
    }
}