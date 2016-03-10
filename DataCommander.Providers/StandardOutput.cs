namespace DataCommander.Providers
{
    using System.Data;
    using System.Data.OleDb;
    using System.IO;
    using System.Text;
    using ADODB;
    using DataCommander.Foundation.Windows.Forms;

    /// <summary>
    /// Summary description for StandardOutput.
    /// </summary>
    internal sealed class StandardOutput : IStandardOutput
    {
        private readonly TextWriter textWriter;
        private readonly QueryForm queryForm;

        public StandardOutput(
            TextWriter textWriter,
            QueryForm queryForm)
        {
            this.textWriter = textWriter;
            this.queryForm = queryForm;
        }

        public TextWriter TextWriter => this.textWriter;

        public void WriteLine(params object[] args)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] != null)
                {
                    string s = args[i].ToString();
                    sb.Append(s);

                    if (i != args.Length - 1)
                    {
                        sb.Append(' ');
                    }
                }
            }

            this.textWriter.WriteLine(sb.ToString());
        }

        public void Write(object arg)
        {
            Recordset rs = arg as Recordset;

            if (rs != null)
            {
                DataSet dataSet = new DataSet();
                OleDbDataAdapter adapter = new OleDbDataAdapter();
                object objRS = arg;

                while (objRS != null)
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable, objRS);
                    dataSet.Tables.Add(dataTable);
                    object recordsAffected;

                    try
                    {
                        objRS = rs.NextRecordset(out recordsAffected);
                        this.textWriter.WriteLine(recordsAffected + " row(s) affected.");
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
                string s = arg.ToString();
                this.textWriter.Write(s);
            }
        }
    }
}