using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DR.Export.Extentions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace DR.Export.ReportExport.ReportCategory {

    public class ReportCategoryExporter : OpentXmlBaseExport {
        private readonly ReportCategoryData data;
        private readonly EExportFile exportFile;

        public ReportCategoryExporter(ReportCategoryData data, EExportFile exportFile) : base() {
            this.data = data;
            this.exportFile = exportFile;
        }

        public static byte[] Export(ReportCategoryData data, EExportFile exportFile) {
            return new ReportCategoryExporter(data, exportFile).Export();
        }

        public override byte[] Export() {
            if (!Directory.Exists("temporary")) Directory.CreateDirectory("temporary");
            string filename = $"temporary/{Guid.NewGuid()}.xlsx";

            try {
                ExportDSToExcel(data, filename, $"BÁO CÁO DANH MỤC\n{data.From:dd/MM/yyyy} - {data.To:dd/MM/yyyy}");

                return base.ExportFile(filename);
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            } finally {
                if (File.Exists(filename))
                    File.Delete(filename);
            }
            return base.ExportFile(filename);
        }

        public void ExportDSToExcel(ReportCategoryData data, string destination, string title) {
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

                var (columnNames, rows, mergeHeaderCels) = GetHeader(table);
                sheetData.Append(rows);
                mergeCells.Append(mergeHeaderCels);
                int[] numberStype = { 5, 2, 2, 2, 7, 7, 7, 7 };
                foreach (DataRow dsrow in table.Rows) {
                    Row newRow = new Row();
                    var columnIndex = 0;
                    foreach (var col in columnNames) {
                        var style = numberStype[columnIndex];
                        Cell cell = new Cell {
                            CellValue = new CellValue(dsrow[col].ToString()),
                            DataType = style != 2 ? CellValues.Number : CellValues.String,
                            StyleIndex = (uint)style,
                        };
                        newRow.AppendChild(cell);
                        columnIndex++;
                    }
                    sheetData.AppendChild(newRow);
                }

                var startFooter = data.Items.Count + 4;
                mergeCells.AppendChild(new MergeCell() { Reference = new StringValue($"A{startFooter}:D{startFooter}") });
                Row footerRow = GetFooter(data);
                sheetData.AppendChild(footerRow);

                //  Merge Duplicate Cell
                var customerCodes = data.Items.Select(o => o.CategoryName).Distinct().ToList();
                var merges = ExcelHelper.AutoMerge(customerCodes, 3, table, columnNames);
                mergeCells.Append(merges);

                Columns cols = ExcelHelper.AutoSize(sheetData);
                sheetPart.Worksheet.InsertBefore(cols, sheetData);
            }
        }

        public override EExportFile GetExportFile() => exportFile;

        public (List<string>, List<Row>, List<MergeCell>) GetHeader(DataTable table) {
            char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            int startHeader = 2;

            List<Row> rows = new List<Row>();
            Row headerRowTop = new Row();
            Row headerRowBottom = new Row();

            List<MergeCell> merges = new List<MergeCell>();
            List<string> columnNames = new List<string>();
            int start = 0;
            foreach (DataColumn column in table.Columns) {
                columnNames.Add(column.ColumnName);
                Cell cell = new Cell {
                    DataType = CellValues.String,
                    CellValue = new CellValue(column.ColumnName),
                    StyleIndex = 1,
                };

                if (start == 9) {
                    var cellTop = new Cell {
                        DataType = CellValues.String,
                        CellValue = new CellValue("Số lượng mua hàng"),
                        StyleIndex = 1,
                    };
                    headerRowTop.AppendChild(cellTop);
                    headerRowBottom.AppendChild(cell);
                } else if (start == 10) {
                    headerRowTop.AppendChild(new Cell { StyleIndex = 1 });
                    headerRowBottom.AppendChild(cell);
                    merges.Add(new MergeCell() { Reference = new StringValue($"J2:K2") });
                } else {
                    headerRowTop.AppendChild(cell);
                    headerRowBottom.AppendChild(new Cell() { StyleIndex = 1 });
                    merges.Add(new MergeCell() { Reference = new StringValue($"{alphabet[start]}{startHeader}:{alphabet[start]}{startHeader + 1}") });
                }
                start++;
            }
            rows.Add(headerRowTop);
            rows.Add(headerRowBottom);
            return (columnNames, rows, merges);
        }

        private Row GetFooter(ReportCategoryData data) {
            Row footerRow = new Row();
            var startFooter = data.Items.Count + 4;

            Cell footerFirstCell = new Cell {
                CellValue = new CellValue("Tổng:"),
                DataType = CellValues.String,
                StyleIndex = 5,
            };
            footerRow.AppendChild(footerFirstCell);

            for (int i = 0; i < 3; i++) {
                Cell tmp = new Cell {
                    StyleIndex = 5,
                };
                footerRow.AppendChild(tmp);
            }

            Cell footerTotalBagCell = new Cell {
                CellValue = new CellValue(data.TotalOnHand),
                DataType = CellValues.Number,
                StyleIndex = 7,
                CellReference = $"E{startFooter}"
            };
            footerRow.AppendChild(footerTotalBagCell);
            Cell footerTotalOrderedKgCell = new Cell {
                CellValue = new CellValue(data.TotalOrdered),
                DataType = CellValues.Number,
                StyleIndex = 7,
                CellReference = $"F{startFooter}"
            };
            footerRow.AppendChild(footerTotalOrderedKgCell);
            Cell footerTotalRefundCell = new Cell {
                CellValue = new CellValue(data.TotalRefund),
                DataType = CellValues.Number,
                StyleIndex = 7,
                CellReference = $"G{startFooter}"
            };
            footerRow.AppendChild(footerTotalRefundCell);

            Cell footerTotalCell = new Cell {
                CellValue = new CellValue(data.Total),
                DataType = CellValues.Number,
                StyleIndex = 7,
                CellReference = $"H{startFooter}"
            };
            footerRow.AppendChild(footerTotalCell);

            return footerRow;
        }
    }
}
