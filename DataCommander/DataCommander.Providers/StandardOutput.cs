using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;
using ADODB;
using DataCommander.Providers.Query;
using Foundation.Windows.Forms;

namespace DataCommander.Providers
{
    /// <summary>
    /// Summary description for StandardOutput.
    /// </summary>
    internal sealed class StandardOutput : IStandardOutput
    {
        private readonly QueryForm _queryForm;

        public StandardOutput(
            TextWriter textWriter,
            QueryForm queryForm)
        {
            TextWriter = textWriter;
            _queryForm = queryForm;
        }

        public TextWriter TextWriter { get; }

        public void WriteLine(params object[] args)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] != null)
                {
                    var s = args[i].ToString();
                    sb.Append(s);

                    if (i != args.Length - 1)
                    {
                        sb.Append(' ');
                    }
                }
            }

            TextWriter.WriteLine(sb.ToString());
        }

        public void Write(object arg)
        {
            var rs = arg as Recordset;

            if (rs != null)
            {
                var dataSet = new DataSet();
                var adapter = new OleDbDataAdapter();
                var objRs = arg;

                while (objRs != null)
                {
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable, objRs);
                    dataSet.Tables.Add(dataTable);

                    try
                    {
                        objRs = rs.NextRecordset(out var recordsAffected);
                        TextWriter.WriteLine(recordsAffected + " row(s) affected.");
                    }
                    catch
                    {
                        objRs = null;
                    }
                }

                _queryForm.Invoke(() => _queryForm.ShowDataSet(dataSet));
            }
            else
            {
                var s = arg.ToString();
                TextWriter.Write(s);
            }
        }
    }
}