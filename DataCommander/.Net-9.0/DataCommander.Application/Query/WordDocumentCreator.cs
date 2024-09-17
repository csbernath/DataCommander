using System;
using System.Data;
using System.IO;
using Microsoft.Office.Interop.Word;
using DataTable = System.Data.DataTable;

namespace DataCommander.Application.Query;

internal static class WordDocumentCreator
{
    public static string CreateWordDocument(DataTable dataTable)
    {
        ApplicationClass application = new ApplicationClass();
        object template = Type.Missing;
        object newTemplate = Type.Missing;
        object documentType = Type.Missing;
        object visible = Type.Missing;
        Document document = application.Documents.Add(ref template, ref newTemplate, ref documentType, ref visible);
        application.Selection.Font.Name = "Tahoma";
        application.Selection.Font.Size = 8;

        Microsoft.Office.Interop.Word.Range range = application.Selection.Range;
        object defaultTableBehaviour = Type.Missing;
        object autoFitBehaviour = WdAutoFitBehavior.wdAutoFitContent;

        int numOfRows = dataTable.Rows.Count + 1;
        int numOfColumns = Math.Min(dataTable.Columns.Count, 63);

        string text = null;
        const string separator = "\t";

        for (int i = 0; i < numOfColumns - 1; i++)
            text += dataTable.Columns[i].ColumnName + separator;

        text += dataTable.Columns[numOfColumns - 1].ColumnName + Environment.NewLine;

        foreach (DataRow dataRow in dataTable.Rows)
        {
            for (int i = 0; i < numOfColumns - 1; i++)
                text += QueryForm.DbValue(dataRow[i]) + separator;

            text += QueryForm.DbValue(dataRow[numOfColumns - 1]) + Environment.NewLine;
        }

        application.Selection.InsertAfter(text);
        object missing = Type.Missing;
        object format = WdTableFormat.wdTableFormatList4;
        Table table = application.Selection.Range.ConvertToTable(ref missing, ref missing, ref missing, ref missing,
            ref format, ref missing, ref missing, ref missing,
            ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing);

        table.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitContent);
        table.Columns.AutoFit();

        foreach (Microsoft.Office.Interop.Word.Column column in table.Columns)
        {
            column.Select();
            column.AutoFit();
        }

        string fileName = Path.GetTempFileName();
        object fileNameObj = fileName;
        object fileFormat = WdSaveFormat.wdFormatRTF;

        document.SaveAs(ref fileNameObj, ref fileFormat, ref missing, ref missing, ref missing, ref missing,
            ref missing, ref missing, ref missing, ref missing, ref missing,
            ref missing, ref missing, ref missing, ref missing, ref missing);
        document.Close(ref missing, ref missing, ref missing);
        application.Quit(ref missing, ref missing, ref missing);

        return fileName;
    }
}