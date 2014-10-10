namespace DataCommander.Foundation.Diagnostics
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Text;

    internal static class StackFrameExtensions
    {
        public static String ToLogString( this StackFrame frame )
        {
            Contract.Requires( frame != null );

            var sb = new StringBuilder();
            var method = frame.GetMethod();
            Type type = method.DeclaringType;
            String typeName = type.FullName;
            String name = method.Name;
            sb.AppendFormat( "   at {0}.{1}(", typeName, name );
            var parameters = method.GetParameters();

            for (Int32 j = 0; j < parameters.Length; j++)
            {
                if (j > 0)
                {
                    sb.Append( ',' );
                }

                var parameter = parameters[ j ];
                type = parameter.ParameterType;
                typeName = type.Name;
                name = parameter.Name;

                sb.AppendFormat( "{0} {1}", typeName, name );
            }

            sb.Append( ')' );

            String fileName = frame.GetFileName();

            if (fileName != null)
            {
                Int32 fileLineNumber = frame.GetFileLineNumber();
                sb.AppendFormat( " in {0}:line {1}", fileName, fileLineNumber );
            }

            return sb.ToString();
        }
    }
}