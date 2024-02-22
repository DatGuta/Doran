using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DR.Export.Extentions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace DR.Export.ProductExporter {

    public class ProductExporter : OpentXmlBaseExport {
        private readonly List<ProductExportData> data;
        private readonly EExportFile exportFile;

        public ProductExporter(List<ProductExportData> data, EExportFile exportFile) : base() {
            this.data = data;
            this.exportFile = exportFile;
        }

        public static byte[] Export(List<ProductExportData> data, EExportFile exportFile) {
            return new ProductExporter(data, exportFile).Export();
        }

        public override byte[] Export() {
            if (!Directory.Exists("temporary")) Directory.CreateDirectory("temporary");
            string filename = $"temporary/{Guid.NewGuid()}.xlsx";
            try {
                CreateExportExcel(data, filename);

                return base.ExportFile(filename);
            } finally {
                if (File.Exists(filename))
                    File.Delete(filename);
            }
        }

        private void CreateExportExcel(List<ProductExportData> data, string fName) {
            using (var workbook = SpreadsheetDocument.Create(fName, SpreadsheetDocumentType.Workbook)) {
                workbook.AddWorkbookPart();
                workbook.WorkbookPart.Workbook = new Workbook {
                    Sheets = new Sheets()
                };

                WorkbookStylesPart stylesPart = workbook.WorkbookPart.AddNewPart<WorkbookStylesPart>();
                stylesPart.Stylesheet = ExcelHelper.GenerateStyleSheet();
                stylesPart.Stylesheet.Save();

                uint sheetId = 1;
                var dataTable = data.ToDataTable();
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

                Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = "Customer" };
                sheets.AppendChild(sheet);

                var (columnNames, headerRow) = GetHeader(table);
                sheetData.AppendChild(headerRow);

                foreach (DataRow dsrow in table.Rows) {
                    bool isFirstCol = true;
                    Row newRow = new Row() { };
                    foreach (var col in columnNames) {
                        var style = isFirstCol ? 5 : 2;
                        Cell cell = new Cell {
                            CellValue = new CellValue(dsrow[col].ToString()),
                            DataType = CellValues.String,
                            StyleIndex = (uint)style,
                        };
                        newRow.AppendChild(cell);
                        isFirstCol = false;
                    }
                    sheetData.AppendChild(newRow);
                }

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
    }
}
