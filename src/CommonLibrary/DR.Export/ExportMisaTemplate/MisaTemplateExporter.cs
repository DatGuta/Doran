using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DR.Export.Extentions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace DR.Export.ExportMisaTemplate {

    public class MisaTemplateExporter : OpentXmlBaseExport {
        private readonly ExportMisaData data;
        private readonly EExportFile exportFile;

        public MisaTemplateExporter(ExportMisaData data, EExportFile exportFile) : base() {
            this.data = data;
            this.exportFile = exportFile;
        }

        public static byte[] Export(ExportMisaData data, EExportFile exportFile) {
            return new MisaTemplateExporter(data, exportFile).Export();
        }

        public override byte[] Export() {
            if (!Directory.Exists("temporary")) Directory.CreateDirectory("temporary");
            string filename = $"temporary/{Guid.NewGuid()}.xlsx";
            try {
                ExportDSToExcel(data, filename);
                return base.ExportFile(filename);
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            } finally {
                if (File.Exists(filename))
                    File.Delete(filename);
            }
            return base.ExportFile(filename);
        }

        public void ExportDSToExcel(ExportMisaData data, string destination) {
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

                //Insert Header
                var (columnNames, row) = GetHeader();
                sheetData.AppendChild(row);

                int[] numberStype = { 9, 9, 2, 2, 2, 2, 2, 2, 7, 7, 7, 7 };
                int[] colAddValue = { 7, 8, 11, 16, 17, 20, 25, 26, 31, 32, 33, 34 };

                foreach (DataRow dsrow in table.Rows) {
                    Row newRow = new Row();
                    var columnIndex = 0;
                    var columnType = 0;
                    foreach (var col in columnNames) {
                        Cell cell = new Cell() {
                            DataType = CellValues.String
                        };

                        if (colAddValue.Contains(columnIndex)) {
                            var style = numberStype[columnType];
                            cell.CellValue = new CellValue(dsrow[col].ToString());
                            cell.DataType = style == 7 ? CellValues.Number : CellValues.String;
                            cell.StyleIndex = (uint)style;
                            columnType++;
                        } else if (columnIndex == 28) {
                            cell.CellValue = new CellValue("1111");
                            cell.DataType = CellValues.Number;
                            cell.StyleIndex = 9;
                        } else if (columnIndex == 29) {
                            cell.CellValue = new CellValue("51111");
                            cell.DataType = CellValues.Number;
                            cell.StyleIndex = 9;
                        } else if (columnIndex == 30) {
                            cell.CellValue = new CellValue("BAO");
                            cell.StyleIndex = 2;
                        } else if (columnIndex == 46) {
                            cell.CellValue = new CellValue("33311");
                            cell.StyleIndex = 11;
                            cell.DataType = CellValues.Number;
                        } else {
                            cell.CellValue = new CellValue("");
                            cell.StyleIndex = 2;
                        }
                        newRow.AppendChild(cell);
                        columnIndex++;
                    }
                    sheetData.AppendChild(newRow);
                }

                Columns cols = ExcelHelper.AutoSize(sheetData, setFirst: true);
                sheetPart.Worksheet.InsertBefore(cols, sheetData);
            }
        }

        public override EExportFile GetExportFile() => exportFile;

        public (List<string>, Row) GetHeader() {
            Row headerRow = new Row();
            List<string> columnNames = new List<string> {
                "Hiển thị trên sổ",                                                             // 1
                "Hình thức bán hàng",                                                           // 2
                "Phương thức thanh toán",                                                       // 3
                "Kiêm phiếu xuất kho",                                                          // 4
                "XK vào khu phi thuế quan và các TH được coi như XK",                           // 5
                "Lập kèm hóa đơn",                                                              // 6
                "Đã lập hóa đơn","Ngày hạch toán (*)",                                          // 7
                "Ngày chứng từ (*)",                                                            // 8
                "Số chứng từ (*)",                                                              // 9
                "Số phiếu xuất",                                                                // 10
                "Lý do xuất",                                                                   // 11
                "Mẫu số HĐ",                                                                    // 12
                "Ký hiệu HĐ",                                                                   // 13
                "Số hóa đơn",                                                                   // 14
                "Ngày hóa đơn",                                                                 // 15
                "Mã khách hàng",                                                                // 16
                "Tên khách hàng",                                                               // 17
                "Địa chỉ",                                                                      // 18
                "Mã số thuế",                                                                   // 19
                "Diễn giải",                                                                    // 20
                "Nộp vào TK",                                                                   // 21
                "NV bán hàng",                                                                  // 22
                "Loại tiền",                                                                    // 23
                "Tỷ giá",                                                                       // 24
                "Mã hàng (*)",                                                                  // 25
                "Tên hàng",                                                                     // 26
                "Hàng khuyến mại",                                                              // 27
                "TK Tiền/Chi phí/Nợ (*)",                                                       // 28
                "TK Doanh thu/Có (*)",                                                          // 29
                "ĐVT",                                                                          // 30
                "Số lượng",                                                                     // 31
                "Đơn giá sau thuế",                                                             // 32
                "Đơn giá",                                                                      // 33
                "Thành tiền",                                                                   // 34
                "Thành tiền quy đổi",                                                           // 35
                "Tỷ lệ CK (%)",                                                                 // 36
                "Tiền chiết khấu",                                                              // 37
                "Tiền chiết khấu quy đổi",                                                      // 38
                "Giá tính thuế XK",                                                             // 40 => 39
                "% thuế XK",                                                                    // 41 => 40
                "Tiền thuế XK",                                                                 // 42 => 41
                "TK thuế XK",                                                                   // 43 => 42
                "Tỷ lệ tính thuế (Thuế suất KHAC)",                                             // 45 => 43
                "Tiền thuế GTGT",                                                               // 46 => 44
                "Tiền thuế GTGT quy đổi",                                                       // 47 => 45
                "TK thuế GTGT",                                                                 // 48 => 46
                "HH không TH trên tờ khai thuế GTGT",                                           // 49 => 47
                "Đơn giá vốn",                                                                  // 53 => 48
                "Tiền vốn",                                                                     // 54 => 49
                "Hàng hóa giữ hộ/bán hộ"                                                        // 55 => 50
            };
            foreach (var column in columnNames) {
                Cell cell = new Cell {
                    DataType = CellValues.String,
                    CellValue = new CellValue(column),
                    StyleIndex = 10,
                };
                headerRow.AppendChild(cell);
            }
            return (columnNames, headerRow);
        }
    }
}
