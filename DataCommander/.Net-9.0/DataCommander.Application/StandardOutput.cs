using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;
using ADODB;
using DataCommander.Application.Query;
using DataCommander.Api;

namespace DataCommander.Application;

internal sealed class StandardOutput(TextWriter textWriter, QueryForm queryForm) : IStandardOutput
{
    private TextWriter TextWriter { get; } = textWriter;

    public void WriteLine(params object[] args)
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] != null)
            {
                string? s = args[i].ToString();
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
        if (arg is Recordset recordset)
        {
            DataSet dataSet = new DataSet();
            OleDbDataAdapter adapter = new OleDbDataAdapter();
            object? objRs = arg;

            while (objRs != null)
            {
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable, objRs);
                dataSet.Tables.Add(dataTable);

                try
                {
                    objRs = recordset.NextRecordset(out object? recordsAffected);
                    TextWriter.WriteLine(recordsAffected + " row(s) affected.");
                }
                catch
                {
                    objRs = null;
                }
            }

            queryForm.Invoke(() => queryForm.ShowDataSet(dataSet));
        }
        else
        {
            string? s = arg.ToString();
            TextWriter.Write(s);
        }
    }
}