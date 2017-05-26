namespace DataCommander.Providers.Query
{
    using System;
    using System.Data;
    using System.IO;
    using Microsoft.Office.Interop.Word;
    using DataTable = System.Data.DataTable;

    internal static class WordDocumentCreator
    {
        public static string CreateWordDocument(DataTable dataTable)
        {
            var application = new ApplicationClass();
            var template = Type.Missing;
            var newTemplate = Type.Missing;
            var documentType = Type.Missing;
            var visible = Type.Missing;
            var document = application.Documents.Add(ref template, ref newTemplate, ref documentType, ref visible);
            application.Selection.Font.Name = "Tahoma";
            application.Selection.Font.Size = 8;

            var range = application.Selection.Range;
            var defaultTableBehaviour = Type.Missing;
            object autoFitBehaviour = WdAutoFitBehavior.wdAutoFitContent;

            var numOfRows = dataTable.Rows.Count + 1;
            var numOfColumns = Math.Min(dataTable.Columns.Count, 63);

            string text = null;
            const string separator = "\t";

            for (var i = 0; i < numOfColumns - 1; i++)
            {
                text += dataTable.Columns[i].ColumnName + separator;
            }

            text += dataTable.Columns[numOfColumns - 1].ColumnName + Environment.NewLine;

            foreach (DataRow dataRow in dataTable.Rows)
            {
                for (var i = 0; i < numOfColumns - 1; i++)
                {
                    text += QueryForm.DbValue(dataRow[i]) + separator;
                }

                text += QueryForm.DbValue(dataRow[numOfColumns - 1]) + Environment.NewLine;
            }

            application.Selection.InsertAfter(text);
            var missing = Type.Missing;
            object format = WdTableFormat.wdTableFormatList4;
            var table = application.Selection.Range.ConvertToTable(ref missing, ref missing, ref missing, ref missing,
                ref format, ref missing, ref missing, ref missing,
                ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing);

            table.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitContent);
            table.Columns.AutoFit();

            foreach (Column column in table.Columns)
            {
                column.Select();
                column.AutoFit();
            }

            var fileName = Path.GetTempFileName();
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
}