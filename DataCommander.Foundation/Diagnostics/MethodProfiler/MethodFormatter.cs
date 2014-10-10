namespace DataCommander.Foundation.Diagnostics
{
    using System;
    using System.Reflection;
    using System.Text;
    using DataCommander.Foundation.Data.SqlClient;
    using DataCommander.Foundation.IO;

    internal sealed class MethodFormatter : IFormatter
    {
        void IFormatter.AppendTo( StringBuilder sb, Object[] args )
        {
            var method = (MethodBase) args[ 0 ];
            Int32 methodId = (Int32) args[ 1 ];
            sb.AppendFormat( "exec MethodProfilerMethod_Add @applicationId,{0},", methodId );
            Type type = method.DeclaringType;
            Assembly assembly = type.Assembly;
            String assemblyName = assembly.GetName().Name;
            String typeName = type.FullName;
            String methodName = method.Name;
            sb.Append( assemblyName.ToTSqlNVarChar() );
            sb.Append( ',' );
            sb.Append( typeName.ToTSqlNVarChar() );
            sb.Append( ',' );
            sb.Append( methodName.ToTSqlNVarChar() );
            sb.AppendLine();
        }
    }
}