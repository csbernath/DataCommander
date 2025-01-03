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
        if (arg is Recordset recordset)
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
                    objRs = recordset.NextRecordset(out var recordsAffected);
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
            var s = arg.ToString();
            TextWriter.Write(s);
        }
    }
}