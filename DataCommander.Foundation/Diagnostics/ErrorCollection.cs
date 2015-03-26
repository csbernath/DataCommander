namespace DataCommander.Foundation.Diagnostics
{
    using System;
    using System.Collections.ObjectModel;
    using System.Text;

    /// <summary>
    /// 
    /// </summary>
    internal sealed class ErrorCollection : Collection<Error>
    {
        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (Error error in this)
            {
                sb.AppendLine(error.ToString());
            }

            return sb.ToString();
        }
    }
}