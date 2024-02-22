using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DR.Export.Extentions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;

namespace DR.Export.ReportExport.ExportStock {

    public class ExportStockExporter : OpentXmlBaseExport {
        private readonly ExportStockData data;
        private readonly EExportFile exportFile;
        private readonly NumberFormatter numberFormatter;

        public ExportStockExporter(ExportStockData data, EExportFile exportFile, NumberFormatInfo info) : base() {
            this.data = data;
            this.exportFile = exportFile;
            numberFormatter = new NumberFormatter(info);
        }

        public static byte[] Export(ExportStockData data, EExportFile exportFile, NumberFormatInfo info) {
            return new ExportStockExporter(data, exportFile, info).Export();
        }

        public override byte[] Export() {
            if (!Directory.Exists("temporary")) Directory.CreateDirectory("temporary");
            string filename = $"temporary/{Guid.NewGuid()}.xlsx";

            try {
                ExportDSToExcel(data, filename, $"BÁO CÁO XUẤT HÀNG\n{data.From} - {data.To}");

                return base.ExportFile(filename);
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            } finally {
                if (File.Exists(filename))
                    File.Delete(filename);
            }
            return base.ExportFile(filename);
        }

        public void ExportDSToExcel(ExportStockData data, string destination, string title) {
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
                Row titleRow = new Row() { Height = new DoubleValue() { Value = 60 }, CustomHeight = true };
                Cell first = new Cell {
                    CellValue = new CellValue(title),
                    DataType = CellValues.String,
                    StyleIndex = 4
                };
                titleRow.AppendChild(first);
                sheetData.AppendChild(titleRow);

                var (columnNames, headerRow) = GetHeader(table);
                sheetData.AppendChild(headerRow);

                int[] numberStype = { 5, 2, 2, 2, 2, 2, 2, 11, 11 };
                foreach (DataRow dsrow in table.Rows) {
                    Row newRow = new Row();
                    var columnIndex = 0;
                    foreach (var col in columnNames) {
                        var style = numberStype[columnIndex];
                        string text = dsrow[col] is decimal value ? numberFormatter.Format(value, columnIndex == 8 ? "N2" : "N") : dsrow[col].ToString();
                        Cell cell = new Cell {
                            CellValue = new CellValue(text),
                            DataType = style == 5 ? CellValues.Number : CellValues.String,
                            StyleIndex = (uint)style,
                        };
                        newRow.AppendChild(cell);
                        columnIndex++;
                    }
                    sheetData.AppendChild(newRow);
                }

                var startFooter = data.Items.Count + 3;
                mergeCells.AppendChild(new MergeCell() { Reference = new StringValue($"{alphabet[0]}{startFooter}:{alphabet[6]}{startFooter}") });
                var footerRow = GetFooter(data);
                sheetData.AppendChild(footerRow);

                Columns cols = ExcelHelper.AutoSize(sheetData);
                sheetPart.Worksheet.InsertBefore(cols, sheetData);
            }
        }

        public override EExportFile GetExportFile() => exportFile;

        public (List<string>, Row) GetHeader(DataTable table) {
            Row headerRow = new Row();
            List<string> columnNames = new List<string>();
            foreach (DataColumn column in table.Columns) {
                columnNames.Add(column.ColumnName);
                Cell cell = new Cell {
                    DataType = CellValues.String,
                    CellValue = new CellValue(column.ColumnName),
                    StyleIndex = 1,
                };
                headerRow.AppendChild(cell);
            }
            return (columnNames, headerRow);
        }

        private Row GetFooter(ExportStockData data) {
            Row footerRow = new Row();
            var startFooter = data.Items.Count + 3;

            Cell footerFirstCell = new Cell {
                CellValue = new CellValue("Tổng:"),
                DataType = CellValues.String,
                StyleIndex = 5,
            };
            footerRow.AppendChild(footerFirstCell);

            for (int i = 0; i < 6; i++) {
                Cell tmp = new Cell {
                    StyleIndex = 5,
                };
                footerRow.AppendChild(tmp);
            }

            Cell footerTotalQuantity = new Cell {
                CellValue = new CellValue(numberFormatter.Format(data.TotalQuantity)),
                DataType = CellValues.String,
                StyleIndex = 11,
                CellReference = $"H{startFooter}"
            };
            footerRow.AppendChild(footerTotalQuantity);
            Cell footerTotalWeight = new Cell {
                CellValue = new CellValue(numberFormatter.Format(data.TotalWeight)),
                DataType = CellValues.String,
                StyleIndex = 11,
                CellReference = $"I{startFooter}"
            };
            footerRow.AppendChild(footerTotalWeight);

            return footerRow;
        }
    }
}
