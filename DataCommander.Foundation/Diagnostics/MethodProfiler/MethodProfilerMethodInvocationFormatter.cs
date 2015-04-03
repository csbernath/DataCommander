namespace DataCommander.Foundation.Diagnostics
{
    using System.Text;
    using DataCommander.Foundation.Data.SqlClient;
    using DataCommander.Foundation.IO;

    internal sealed class MethodProfilerMethodInvocationFormatter : IFormatter
    {
        void IFormatter.AppendTo( StringBuilder sb, object[] args )
        {
            var item = (MethodInvocation) args[ 0 ];
            MethodInvocation parent = item.Parent;
            int? parentId = parent != null ? parent.Id : (int?) null;
            sb.AppendFormat( "exec MethodProfilerMethodInvocation_Add @applicationId,{0},{1},{2},{3},{4}\r\n",
                item.Id,
                parentId.ToTSqlInt(),
                item.MethodId,
                item.BeginTime,
                item.EndTime );
        }
    }
}