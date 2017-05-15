namespace DataCommander.Providers
{
    using System.Data;
    using System.Data.OleDb;
    using System.IO;
    using System.Text;
    using ADODB;
    using DataCommander.Foundation.Windows.Forms;
    using Query;

    /// <summary>
    /// Summary description for StandardOutput.
    /// </summary>
    internal sealed class StandardOutput : IStandardOutput
    {
        private readonly QueryForm queryForm;

        public StandardOutput(
            TextWriter textWriter,
            QueryForm queryForm)
        {
            this.TextWriter = textWriter;
            this.queryForm = queryForm;
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

            this.TextWriter.WriteLine(sb.ToString());
        }

        public void Write(object arg)
        {
            var rs = arg as Recordset;

            if (rs != null)
            {
                var dataSet = new DataSet();
                var adapter = new OleDbDataAdapter();
                var objRS = arg;

                while (objRS != null)
                {
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable, objRS);
                    dataSet.Tables.Add(dataTable);
                    object recordsAffected;

                    try
                    {
                        objRS = rs.NextRecordset(out recordsAffected);
                        this.TextWriter.WriteLine(recordsAffected + " row(s) affected.");
                    }
                    catch
                    {
                        objRS = null;
                    }
                }

                this.queryForm.Invoke( () => this.queryForm.ShowDataSet( dataSet ) );
            
            }
            else
            {
                var s = arg.ToString();
                this.TextWriter.Write(s);
            }
        }
    }
}