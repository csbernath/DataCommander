namespace DataCommander.Foundation.Diagnostics.MethodProfiler
{
    using System;
    using System.Reflection;
    using System.Text;
    using DataCommander.Foundation.Data.SqlClient;
    using DataCommander.Foundation.IO;

    internal sealed class MethodFormatter : IFormatter
    {
        void IFormatter.AppendTo( StringBuilder sb, object[] args )
        {
            var method = (MethodBase) args[ 0 ];
            int methodId = (int) args[ 1 ];
            sb.AppendFormat( "exec MethodProfilerMethod_Add @applicationId,{0},", methodId );
            Type type = method.DeclaringType;
            Assembly assembly = type.Assembly;
            string assemblyName = assembly.GetName().Name;
            string typeName = type.FullName;
            string methodName = method.Name;
            sb.Append( assemblyName.ToTSqlNVarChar() );
            sb.Append( ',' );
            sb.Append( typeName.ToTSqlNVarChar() );
            sb.Append( ',' );
            sb.Append( methodName.ToTSqlNVarChar() );
            sb.AppendLine();
        }
    }
}