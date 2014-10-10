namespace DataCommander.Foundation.Text
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using DataCommander.Foundation.Configuration;

    /// <exclude/>
    public static class StringHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="col"></param>
        /// <param name="colWidth"></param>
        /// <param name="alignRight"></param>
        /// <returns></returns>
        public static String FormatColumn(
            String col,
            Int32 colWidth,
            Boolean alignRight )
        {
            Int32 length = col != null ? col.Length : 0;
            Int32 spaceLen = colWidth - length;
            String formatted;

            if (spaceLen >= 0)
            {
                var space = new String( ' ', spaceLen );

                if (alignRight)
                {
                    formatted = space + col;
                }
                else
                {
                    formatted = col + space;
                }
            }
            else
            {
                formatted = col.Substring( 0, colWidth );
            }

            return formatted;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="index"></param>
        /// <param name="ch"></param>
        public static unsafe void SetChar( String s, Int32 index, Char ch )
        {
            Contract.Requires( index >= 0 );
            Contract.Requires( index < s.Length );

            fixed (Char* p = s)
            {
                p[ index ] = ch;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        public static unsafe void ToLower( String s )
        {
            fixed (Char* pfixed = s)
            {
                for (Char* p = pfixed; *p != 0; p++)
                {
                    *p = Char.ToLower( *p, CultureInfo.CurrentCulture );
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        public static unsafe void ToUpper( String s )
        {
            fixed (Char* pfixed = s)
            {
                for (Char* p = pfixed; *p != 0; p++)
                {
                    *p = Char.ToUpper( *p, CultureInfo.CurrentCulture );
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textWriter"></param>
        /// <param name="obj"></param>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        public static void WriteMethod(
            TextWriter textWriter,
            Object obj,
            String methodName,
            params Object[] parameters )
        {
            Contract.Requires( textWriter != null );
            Contract.Requires( obj != null );

            Type type = obj.GetType();
            MethodInfo methodInfo = type.GetMethod( methodName );
            ParameterInfo[] parameterInfos = methodInfo.GetParameters();

            String typeName = TypeNameCollection.GetTypeName( methodInfo.ReturnType );

            String line = typeName + " " + methodName + "(" + Environment.NewLine;

            Int32 length = Math.Min( parameters.Length, parameterInfos.Length );

            for (Int32 i = 0; i < length; i++)
            {
                typeName = TypeNameCollection.GetTypeName( parameterInfos[ i ].ParameterType );

                line +=
                    "  " + typeName + " " +
                    parameterInfos[ i ].Name + " = " +
                    parameters[ i ];

                if (i < length - 1)
                {
                    line += "," + Environment.NewLine;
                }
            }

            line += ')';

            textWriter.Write( line );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="nullValue"></param>
        /// <returns></returns>
        public static Boolean ParseBoolean( String value, Boolean nullValue )
        {
            Boolean b = value != null && value.Length > 0 ? Boolean.Parse( value ) : nullValue;
            return b;
        }
    }
}