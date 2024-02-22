using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DR.Export.Extentions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace DR.Export.ReportExport.ReportHistoryStock {
    public class ReportHistoryStockExporter : OpentXmlBaseExport {
        private readonly ReportHistoryStockData data;
        private readonly EExportFile exportFile;

        public ReportHistoryStockExporter(ReportHistoryStockData data, EExportFile exportFile) : base() {
            this.data = data;
            this.exportFile = exportFile;
        }

        public static byte[] Export(ReportHistoryStockData data, EExportFile exportFile) {
            return new ReportHistoryStockExporter(data, exportFile).Export();
        }

        public override byte[] Export() {
            if (!Directory.Exists("temporary")) Directory.CreateDirectory("temporary");
            string filename = $"temporary/{Guid.NewGuid()}.xlsx";

            try {
                var sb = new StringBuilder();
                sb.AppendLine("BÁO CÁO LỊCH SỬ TỒN KHO");
                sb.AppendLine($"{data.ProductName} - {data.WarehouseName}");
                sb.AppendLine($"{data.From} - {data.To}");

                ExportDSToExcel(data, filename, sb.ToString());

                return base.ExportFile(filename);
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            } finally {
                if (File.Exists(filename))
                    File.Delete(filename);
            }
            return base.ExportFile(filename);
        }

        public override EExportFile GetExportFile() => exportFile;

        public void ExportDSToExcel(ReportHistoryStockData data, string destination, string title) {
            char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

            using (var workbook = SpreadsheetDocument.Create(destination, SpreadsheetDocumentType.Workbook)) {
                workbook.AddWorkbookPart();
                workbook.WorkbookPart.Workbook = new Workbook {
                    Sheets = new Sheets()
                };

                WorkbookStylesPart stylesPart = workbook.WorkbookPart.AddNewPart<WorkbookStylesPart>();
                stylesPart.Stylesheet = ExcelHelper.GenerateStyleSheet();
                stylesPart.Stylesheet.Save();

                uint sheetId = 1;
                var dataTable = data.Items.ToDataTable();
                var ds = new DataSet();
                ds.Tables.Add(dataTable);

                var table = ds.Tables[0];
                var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                var sheetData = new SheetData();
                sheetPart.Worksheet = new Worksheet();
                sheetPart.Worksheet.AppendChild(sheetData);

                Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<Sheets>();
                string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

                if (sheets.Elements<Sheet>().Any()) {
                    sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                }

                Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = "Report" };
                sheets.AppendChild(sheet);

                var workSheet = sheetPart.Worksheet;
                MergeCells mergeCells;
                if (workSheet.Elements<MergeCells>().Any()) {
                    mergeCells = workSheet.Elements<MergeCells>().First();
                } else {
                    mergeCells = new MergeCells();
                    workSheet.InsertAfter(mergeCells, workSheet.Elements<SheetData>().First());
                }

                //Insert Header
                mergeCells.AppendChild(new MergeCell() { Reference = new StringValue($"{alphabet[0]}1:{alphabet[table.Columns.Count - 1]}1") });
                Row titleRow = new Row() { Height = new DoubleValue() { Value = 90 }, CustomHeight = true };
                titleRow.AppendChild(new Cell {
                    CellValue = new CellValue(title),
                    DataType = CellValues.String,
                    StyleIndex = 4
                });
                sheetData.AppendChild(titleRow);

                var (columnNames, headerRow) = GetHeader(table);
                sheetData.AppendChild(headerRow);

                var styles = new uint[] { 2, 2, 2, 2, 11, 11, 11, 11 };
                var body = GetBody(table.Rows, styles, columnNames);
                sheetData.Append(body);

                Columns cols = ExcelHelper.AutoSize(sheetData, true);
                var firstCol = cols.GetFirstChild<Column>();
                firstCol.Width = 15;
                sheetPart.Worksheet.InsertBefore(cols, sheetData);
            }
        }

        public (List<string>, Row) GetHeader(DataTable table) {
            Row headerRow = new Row();
            List<string> columnNames = new List<string>();
            foreach (DataColumn column in table.Columns) {
                columnNames.Add(column.ColumnName);
                headerRow.AppendChild(new Cell {
                    DataType = CellValues.String,
                    CellValue = new CellValue(column.ColumnName),
                    StyleIndex = 1,
                });
            }
            return (columnNames, headerRow);
        }

        public static Row[] GetBody(DataRowCollection data, uint[] styles, List<string> columnNames) {
            var rows = new List<Row>();
            foreach (DataRow dsrow in data) {
                Row newRow = new Row() { };
                var columnIndex = 0;
                foreach (var col in columnNames) {
                    var style = styles[columnIndex];
                    Cell cell = new Cell {
                        CellValue = new CellValue(dsrow[col].ToString()),
                        DataType = CellValues.String,
                        StyleIndex = style,
                    };
                    newRow.AppendChild(cell);
                    columnIndex++;
                }
                rows.Add(newRow);
            }
            return rows.ToArray();
        }
    }
}
