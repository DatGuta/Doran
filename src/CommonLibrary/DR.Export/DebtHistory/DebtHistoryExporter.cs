﻿using System.Data;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DR.Export.Extentions;

namespace DR.Export.DebtHistory {

    public class DebtHistoryExporter : OpentXmlBaseExport {
        private readonly char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        private readonly DebtHistoryReport data;
        private readonly EExportFile exportFile;

        public DebtHistoryExporter(DebtHistoryReport data, EExportFile exportFile) : base() {
            this.data = data;
            this.exportFile = exportFile;
        }

        public static byte[] Export(DebtHistoryReport data, EExportFile exportFile) {
            return new DebtHistoryExporter(data, exportFile).Export();
        }

        public override byte[] Export() {
            if (!Directory.Exists("temporary")) Directory.CreateDirectory("temporary");
            string filename = $"temporary/{Guid.NewGuid()}.xlsx";
            try {
                CreateExportExcel(data.Items, filename, $"CHI TIẾT CÔNG NỢ\n{data.From:dd/MM/yyyy} - {data.To:dd/MM/yyyy}");

                return base.ExportFile(filename);
            } finally {
                if (File.Exists(filename))
                    File.Delete(filename);
            }
        }

        private void CreateExportExcel(List<DebtHistoryItem> data, string fName, string title) {
            using (var workbook = SpreadsheetDocument.Create(fName, SpreadsheetDocumentType.Workbook)) {
                workbook.AddWorkbookPart();
                workbook.WorkbookPart.Workbook = new Workbook { Sheets = new Sheets() };

                //Khởi tạo style cho excel
                WorkbookStylesPart stylesPart = workbook.WorkbookPart.AddNewPart<WorkbookStylesPart>();
                stylesPart.Stylesheet = ExcelHelper.GenerateStyleSheet();
                stylesPart.Stylesheet.Save();

                //Khỏi tạo dataset cho dữ liệu đầu vào
                uint sheetId = 1;
                var dataTable = data.ToDataTable();
                var ds = new DataSet();
                ds.Tables.Add(dataTable);
                var table = ds.Tables[0];

                //Tạo WorksheetPart data
                var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                var sheetData = new SheetData();
                sheetPart.Worksheet = new Worksheet();
                sheetPart.Worksheet.AppendChild(sheetData);

                //Tạo sheet data
                Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<Sheets>();
                string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);
                if (sheets.Elements<Sheet>().Any()) {
                    sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                }
                Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = "Chi tiết công nợ" };
                sheets.AppendChild(sheet);

                //Khỏi tạo mergercells cho data
                var workSheet = sheetPart.Worksheet;
                MergeCells mergeCells;
                if (workSheet.Elements<MergeCells>().Any()) {
                    mergeCells = workSheet.Elements<MergeCells>().First();
                } else {
                    mergeCells = new MergeCells();
                    workSheet.InsertAfter(mergeCells, workSheet.Elements<SheetData>().First());
                }

                //Tạo Title
                mergeCells.AppendChild(new MergeCell() { Reference = new StringValue($"{alphabet[0]}1:{alphabet[table.Columns.Count - 1]}1") });
                Row titleRow = new Row() { Height = new DoubleValue() { Value = 60 }, CustomHeight = true };
                Cell first = new Cell {
                    CellValue = new CellValue(title),
                    DataType = CellValues.String,
                    StyleIndex = 4
                };
                titleRow.AppendChild(first);
                sheetData.AppendChild(titleRow);

                //Tạo Header table
                var (columnNames, rows, mergeHeaderCels) = GetHeader(table);
                sheetData.Append(rows);
                mergeCells.Append(mergeHeaderCels);

                // Tạo rows
                int[] numberStype = { 5, 9, 2, 2, 2, 11, 11, 11, 11, 11, 2 };
                foreach (DataRow dsrow in table.Rows) {
                    Row newRow = new Row();
                    var columnIndex = 0;
                    foreach (var col in columnNames) {
                        var style = numberStype[columnIndex];
                        Cell cell = new Cell {
                            CellValue = new CellValue(dsrow[col].ToString()),
                            DataType = style == 5 ? CellValues.Number : CellValues.String,
                            StyleIndex = (uint)style,
                        };
                        newRow.AppendChild(cell);
                        columnIndex++;
                    }
                    sheetData.AppendChild(newRow);
                }

                Columns cols = ExcelHelper.AutoSize(sheetData);
                sheetPart.Worksheet.InsertBefore(cols, sheetData);
            }
        }

        public override EExportFile GetExportFile() => exportFile;

        public (List<string>, List<Row>, List<MergeCell>) GetHeader(DataTable table) {
            int startHeader = 2;

            List<Row> rows = new List<Row>();
            Row headerRowTop = new Row();
            Row headerRowBottom = new Row();

            List<MergeCell> merges = new List<MergeCell>();
            List<string> columnNames = new List<string>();
            int start = 0;
            foreach (DataColumn column in table.Columns) {
                columnNames.Add(column.ColumnName);

                var textTop = "";
                switch (start) {
                    case 5:
                        textTop = "Số tiền";
                        merges.Add(new MergeCell() { Reference = new StringValue($"{alphabet[start]}{startHeader}:{alphabet[start + 1]}{startHeader}") });
                        break;


                    case 8:
                        textTop = "Còn lại";
                        merges.Add(new MergeCell() { Reference = new StringValue($"{alphabet[start]}{startHeader}:{alphabet[start + 1]}{startHeader}") });
                        break;

                    default:
                        if (start != 6 && start != 9) {
                            textTop = column.ColumnName;
                            merges.Add(new MergeCell() { Reference = new StringValue($"{alphabet[start]}{startHeader}:{alphabet[start]}{startHeader + 1}") });
                        }
                        break;
                }

                Cell cellTop = new Cell {
                    DataType = CellValues.String,
                    CellValue = new CellValue(textTop),
                    StyleIndex = 1,
                };

                Cell cellBottom = new Cell {
                    DataType = CellValues.String,
                    CellValue = new CellValue(column.ColumnName),
                    StyleIndex = 1,
                };

                headerRowTop.AppendChild(cellTop);
                headerRowBottom.AppendChild(cellBottom);
                start++;
            }
            rows.Add(headerRowTop);
            rows.Add(headerRowBottom);
            return (columnNames, rows, merges);
        }
    }
}
