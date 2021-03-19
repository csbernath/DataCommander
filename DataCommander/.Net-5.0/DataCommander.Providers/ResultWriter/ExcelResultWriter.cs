using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using DataCommander.Providers2.Connection;
using Foundation.Assertions;
using Foundation.Data;
using OfficeOpenXml;

namespace DataCommander.Providers.ResultWriter
{
    internal sealed class ExcelResultWriter : IResultWriter
    {
        private IProvider _provider;
        private readonly Action<InfoMessage> _addInfoMessage;
        private readonly IResultWriter _logResultWriter;
        private ExcelPackage _excelPackage;
        private ExcelWorksheet _excelWorksheet;
        private int _rowCount;

        public ExcelResultWriter(IProvider provider, Action<InfoMessage> addInfoMessage)
        {
            Assert.IsNotNull(provider);
            Assert.IsNotNull(addInfoMessage);

            _provider = provider;
            _addInfoMessage = addInfoMessage;
            _logResultWriter = new LogResultWriter(addInfoMessage);
        }

        #region IResultWriter Members

        void IResultWriter.Begin(IProvider provider)
        {
            _logResultWriter.Begin(provider);
            _provider = provider;
        }

        void IResultWriter.BeforeExecuteReader(AsyncDataAdapterCommand command) => _logResultWriter.BeforeExecuteReader(command);

        void IResultWriter.AfterExecuteReader(int fieldCount)
        {
            _logResultWriter.AfterExecuteReader(fieldCount);

            var fileName = Path.GetTempFileName() + ".xlsx";
            _excelPackage = new ExcelPackage(new FileInfo(fileName));
        }

        void IResultWriter.AfterCloseReader(int affectedRows) => _logResultWriter.AfterCloseReader(affectedRows);

        void IResultWriter.WriteTableBegin(DataTable schemaTable)
        {
            _logResultWriter.WriteTableBegin(schemaTable);
            CreateTable(schemaTable);
        }

        void IResultWriter.FirstRowReadBegin() => _logResultWriter.FirstRowReadBegin();
        void IResultWriter.FirstRowReadEnd(string[] dataTypeNames) => _logResultWriter.FirstRowReadEnd(dataTypeNames);

        void IResultWriter.WriteRows(object[][] rows, int rowCount)
        {
            _logResultWriter.WriteRows(rows, rowCount);

            var cells = _excelWorksheet.Cells;

            for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                var row = rows[rowIndex];

                for (var columnIndex = 0; columnIndex < row.Length; columnIndex++)
                {
                    cells[_rowCount + rowIndex, columnIndex + 1].Value = row[columnIndex];
                }
            }

            _rowCount += rowCount;
        }

        void IResultWriter.WriteTableEnd() => _logResultWriter.WriteTableEnd();

        void IResultWriter.WriteParameters(IDataParameterCollection parameters)
        {
        }

        void IResultWriter.End()
        {
            _logResultWriter.End();

            _excelPackage.Save();

            Process.Start(_excelPackage.File.FullName);
        }

        #endregion

        private void CreateTable(DataTable schemaTable)
        {
            var worksheets = _excelPackage.Workbook.Worksheets;
            var tableName = $"Table{worksheets.Count + 1}";
            _excelWorksheet = worksheets.Add(tableName);
            var cells = _excelWorksheet.Cells;
            var columnIndex = 1;

            foreach (DataRow schemaRow in schemaTable.Rows)
            {
                var dataColumnSchema = FoundationDbColumnFactory.Create(schemaRow);
                var columnName = dataColumnSchema.ColumnName;
                var columnSize = dataColumnSchema.ColumnSize;
                var dataType = _provider.GetColumnType(dataColumnSchema);

                var cell = cells[1, columnIndex];
                cell.Value = columnName;
                cell.Style.Font.Bold = true;

                columnIndex++;
            }

            _excelWorksheet.View.FreezePanes(2, 1);
            _rowCount = 2;
        }
    }
}