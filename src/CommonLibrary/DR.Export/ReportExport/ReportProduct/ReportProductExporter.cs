using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DR.Export.Extentions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace DR.Export.ReportExport.ReportProduct {

    public class ReportProductExporter : OpentXmlBaseExport {
        private readonly ReportProductData data;
        private readonly EExportFile exportFile;

        public ReportProductExporter(ReportProductData data, EExportFile exportFile) : base() {
            this.data = data;
            this.exportFile = exportFile;
        }

        public static byte[] Export(ReportProductData data, EExportFile exportFile) {
            return new ReportProductExporter(data, exportFile).Export();
        }

        public override byte[] Export() {
            if (!Directory.Exists("temporary")) Directory.CreateDirectory("temporary");
            string filename = $"temporary/{Guid.NewGuid()}.xlsx";

            try {
                ExportDSToExcel(data, filename, $"BÁO CÁO BÁN HÀNG (SẢN PHẨM)\n{data.From:dd/MM/yyyy} - {data.To:dd/MM/yyyy}");

                return base.ExportFile(filename);
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            } finally {
                if (File.Exists(filename))
                    File.Delete(filename);
            }
            return base.ExportFile(filename);
        }

        public void ExportDSToExcel(ReportProductData data, string destination, string title) {
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

                int[] styleOfCol = { 5, 2, 2, 2, 2, 2, 2, 2, 2, 2, 7, 7, 7, 7, 7, 7, 7 };
                var body = ExcelHelper.GetBody(table.Rows, styleOfCol, columnNames);
                sheetData.Append(body);

                var (footerRow, mergeCell) = GetFooter(data);
                mergeCells.AppendChild(mergeCell);
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

        private (Row, MergeCell) GetFooter(ReportProductData data) {
            Row footerRow = new Row();
            var startFooter = data.Items.Count + 3;
            var mergeCell = new MergeCell() { Reference = new StringValue($"A{startFooter}:K{startFooter}") };

            Cell footerFirstCell = new Cell {
                CellValue = new CellValue("Tổng:"),
                DataType = CellValues.String,
                StyleIndex = 5,
            };
            footerRow.AppendChild(footerFirstCell);

            for (int i = 0; i < 10; i++) {
                Cell tmp = new Cell {
                    StyleIndex = 5,
                };
                footerRow.AppendChild(tmp);
            }
            var cells = new List<Cell>() {
                new Cell {
                    CellValue = new CellValue(data.SumOrdered.ToString()),
                    DataType = CellValues.Number,
                    StyleIndex = 7,
                    CellReference = $"L{startFooter}"
                }, new Cell {
                    CellValue = new CellValue(data.SumExported.ToString()),
                    DataType = CellValues.Number,
                    StyleIndex = 7,
                    CellReference = $"M{startFooter}"
                }, new Cell {
                    CellValue = new CellValue(data.SumRefunded.ToString()),
                    DataType = CellValues.Number,
                    StyleIndex = 7,
                    CellReference = $"N{startFooter}"
                }, new Cell {
                    StyleIndex = 7,
                    DataType = CellValues.Number,
                    CellReference = $"O{startFooter}"
                }, new Cell {
                    CellValue = new CellValue(data.Total.ToString()),
                    DataType = CellValues.Number,
                    StyleIndex = 7,
                    CellReference = $"P{startFooter}"
                }
            };
            footerRow.Append(cells);

            return (footerRow, mergeCell);
        }
    }
}
