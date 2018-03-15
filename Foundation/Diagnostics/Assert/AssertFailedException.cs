#if FOUNDATION_3_5

namespace Foundation.Diagnostics
{
    using System;
    using System.Text;

    /// <summary>
    /// 
    /// </summary>
    public class AssertFailedException : Exception
    {
        private readonly AssertMessage message;

        internal AssertFailedException( AssertMessage message )
        {
            this.message = message;
        }

        /// <summary>
        /// 
        /// </summary>
        public AssertMessage AssertMessage
        {
            get
            {
                return this.message;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override string Message
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendFormat( "AssertFailedException({0}))", this.message.Name );
                sb.AppendLine();

                AssertMessageParameterCollection parameters = this.message.Parameters;

                if (parameters != null)
                {
                    foreach (AssertMessageParameter parameter in parameters)
                    {
                        string valueString = ToString( parameter.Value );
                        sb.AppendFormat( "{0} = {1}", parameter.Name, valueString );
                        sb.AppendLine();
                    }
                }

                string message = sb.ToString();
                return message;
            }
        }

        private static string ToString( object value )
        {
            string s;

            if (value != null)
            {
                s = value.ToString();
            }
            else
            {
                s = "null";
            }

            return s;
        }
    }
}

#endif