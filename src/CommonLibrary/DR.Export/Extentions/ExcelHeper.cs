using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DR.Export.Extentions {
    public static class ExcelHelper {

        public static Columns AutoSize(SheetData sheetData, bool setFirst = false) {
            var maxColWidth = GetMaxCharacterWidth(sheetData);

            Columns columns = new Columns();
            double maxWidth = 7;
            foreach (var item in maxColWidth) {
                double width = Math.Truncate((item.Value * maxWidth + 5) / maxWidth * 256) / 256;
                Column col = new Column() { BestFit = true, Min = (uint)(item.Key + 1), Max = (uint)(item.Key + 1), CustomWidth = true, Width = (DoubleValue)width };
                if (item.Key + 1 == 1 && !setFirst) {
                    col.Width = (DoubleValue)5;
                }
                columns.AppendChild(col);
            }
            return columns;
        }

        private static Dictionary<int, int> GetMaxCharacterWidth(SheetData sheetData) {
            Dictionary<int, int> maxColWidth = new Dictionary<int, int>();
            var rows = sheetData.Elements<Row>();
            uint[] numberStyles = new uint[] { 5, 6, 7, 8 };
            uint[] boldStyles = new uint[] { 1, 2, 3, 4, 6, 7, 8 };
            foreach (var r in rows) {
                var cells = r.Elements<Cell>().ToArray();

                for (int i = 0; i < cells.Length; i++) {
                    var cell = cells[i];
                    var cellValue = cell.CellValue == null ? string.Empty : cell.CellValue.InnerText;
                    var cellTextLength = cellValue.Length;

                    if (cell.StyleIndex != null && numberStyles.Contains(cell.StyleIndex)) {
                        int thousandCount = (int)Math.Truncate((double)cellTextLength / 4);
                        cellTextLength += 3 + thousandCount;
                    }

                    if (cell.StyleIndex != null && boldStyles.Contains(cell.StyleIndex)) {
                        cellTextLength += 1;
                    }

                    if (maxColWidth.ContainsKey(i)) {
                        var current = maxColWidth[i];
                        cellTextLength += 5;
                        if (cellTextLength > current) {
                            maxColWidth[i] = cellTextLength;
                        }
                    } else {
                        maxColWidth.Add(i, cellTextLength);
                    }
                }
            }

            return maxColWidth;
        }

        public static Stylesheet GenerateStyleSheet() {
            return new Stylesheet(GetFonts(), GetFills(), GetBorders(), GetCellFormats());
        }

        private static Fonts GetFonts() {
            return new Fonts(
                new Font(
                    new FontSize() { Val = 13 },
                    new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                    new FontName() { Val = "Times New Roman" }),//Index 0
                new Font(
                    new FontSize() { Val = 20 },
                    new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                    new FontName() { Val = "Times New Roman" }), //Index 1
                new Font(
                    new Bold(),
                    new FontSize() { Val = 13 },
                    new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                    new FontName() { Val = "Times New Roman" }),//Index 2
                new Font(
                    new Bold(),
                    new FontSize() { Val = 20 },
                    new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                    new FontName() { Val = "Times New Roman" }) //Index 3

            );
        }

        private static Fills GetFills() {
            return new Fills(
                        new Fill( // Index 0 - The default fill.
                            new PatternFill() { PatternType = PatternValues.None }, null),
                        new Fill( // Index 1 - The default fill of gray 125 (required)
                            new PatternFill() { PatternType = PatternValues.Gray125 }, null),
                        new Fill( // Index 2 - The Green fill.
                            new PatternFill() {
                                PatternType = PatternValues.Solid,
                                ForegroundColor = new ForegroundColor() { Rgb = new HexBinaryValue() { Value = "C6E0B4" } }
                            }, null
                        ),
                        new Fill( // Index 3 - The Blue fill.
                            new PatternFill() {
                                PatternType = PatternValues.Solid,
                                ForegroundColor = new ForegroundColor() { Rgb = new HexBinaryValue() { Value = "DDEBF7" } }
                            }, null
                        ),
                        new Fill( // Index 4 - The Purple fill.
                            new PatternFill() {
                                PatternType = PatternValues.Solid,
                                ForegroundColor = new ForegroundColor() { Rgb = new HexBinaryValue() { Value = "CCCCFF" } }
                            }, null
                        )
                    );
        }

        private static CellFormats GetCellFormats() {
            return new CellFormats(
                    // Index 0 - The default
                    new CellFormat() { FontId = 0, FillId = 0, BorderId = 0 },
                    // Index 1 - Green Fill
                    new CellFormat(new Alignment() {
                        WrapText = true,
                        Vertical = VerticalAlignmentValues.Center,
                        Horizontal = HorizontalAlignmentValues.Center,
                    }, null) { FontId = 0, FillId = 2, BorderId = 1, ApplyFill = true, ApplyBorder = true, ApplyAlignment = true },
                    // Index 2 - Alignment
                    new CellFormat(new Alignment() {
                        Vertical = VerticalAlignmentValues.Center,
                        WrapText = false,
                    }, null) { FontId = 0, FillId = 0, BorderId = 1, ApplyAlignment = true, ApplyFill = true, ApplyBorder = true },
                    // Index 3 - Green Fill
                    new CellFormat() { FontId = 0, FillId = 0, BorderId = 1, ApplyBorder = true },
                    // Index 4 - Title Fill
                    new CellFormat(new Alignment() {
                        WrapText = true,
                        Vertical = VerticalAlignmentValues.Center,
                        Horizontal = HorizontalAlignmentValues.Center
                    }, null) { FontId = 3, FillId = 0, BorderId = 1, ApplyBorder = true, ApplyAlignment = true },
                    // Index 5 - Alignment Center Fill
                    new CellFormat(new Alignment() {
                        WrapText = true,
                        Vertical = VerticalAlignmentValues.Center,
                        Horizontal = HorizontalAlignmentValues.Center,
                    }, null) { FontId = 0, FillId = 0, BorderId = 1, ApplyBorder = true, ApplyAlignment = true },
                    // Index 6 - Alignment Left Fill
                    new CellFormat(new Alignment() {
                        WrapText = true,
                        Vertical = VerticalAlignmentValues.Center,
                        Horizontal = HorizontalAlignmentValues.Left
                    }, null) { FontId = 0, FillId = 0, BorderId = 1, ApplyBorder = true, ApplyAlignment = true },
                    // Index 7 - Alignment Right Fill
                    new CellFormat(new Alignment() {
                        WrapText = true,
                        Vertical = VerticalAlignmentValues.Center,
                        Horizontal = HorizontalAlignmentValues.Right,
                    }, null) { FontId = 0, FillId = 0, BorderId = 1, ApplyBorder = true, ApplyAlignment = true, ApplyNumberFormat = true, NumberFormatId = 3 },
                    // Index 8 - Blue Fill, Center
                    new CellFormat(new Alignment() {
                        WrapText = true,
                        Vertical = VerticalAlignmentValues.Center,
                        Horizontal = HorizontalAlignmentValues.Center,
                    }, null) { FontId = 2, FillId = 3, BorderId = 1, ApplyFill = true, ApplyBorder = true, ApplyAlignment = true },
                    // Index 9 - Text Center
                    new CellFormat(new Alignment() {
                        WrapText = true,
                        Vertical = VerticalAlignmentValues.Center,
                        Horizontal = HorizontalAlignmentValues.Center,
                    }, null) { FontId = 0, FillId = 0, BorderId = 1, ApplyAlignment = true, ApplyFill = true, ApplyBorder = true },
                    // Index 10 - Text Purple Center
                    new CellFormat(new Alignment() {
                        WrapText = false,
                        Vertical = VerticalAlignmentValues.Center,
                        Horizontal = HorizontalAlignmentValues.Center,
                    }, null) { FontId = 0, FillId = 4, BorderId = 1, ApplyAlignment = true, ApplyFill = true, ApplyBorder = true },

                    // Index 11 - Text Right
                    new CellFormat(new Alignment() {
                        WrapText = true,
                        Vertical = VerticalAlignmentValues.Center,
                        Horizontal = HorizontalAlignmentValues.Right,
                    }, null) { FontId = 0, FillId = 0, BorderId = 1, ApplyAlignment = true, ApplyFill = true, ApplyBorder = true }
                );
        }

        private static Borders GetBorders() {
            return new Borders(
                    // Index 0 - The default border.
                    new Border(
                        new LeftBorder(),
                        new RightBorder(),
                        new TopBorder(),
                        new BottomBorder(),
                        new DiagonalBorder()
                    ),
                    // Index 1 - Applies a Left, Right, Top, Bottom border to a cell
                    new Border(
                        new LeftBorder(new Color() { Rgb = new HexBinaryValue { Value = "000000" } }, null) { Style = BorderStyleValues.Thin },
                        new RightBorder(new Color() { Rgb = new HexBinaryValue { Value = "000000" } }, null) { Style = BorderStyleValues.Thin },
                        new TopBorder(new Color() { Rgb = new HexBinaryValue { Value = "000000" } }, null) { Style = BorderStyleValues.Thin },
                        new BottomBorder(new Color() { Rgb = new HexBinaryValue { Value = "000000" } }, null) { Style = BorderStyleValues.Thin },
                        new DiagonalBorder()
                    )
                );
        }

        public static List<MergeCell> AutoMerge(List<string> codes, int startRow, DataTable data, List<string> columnNames, int start = 1) {
            var merges = new List<MergeCell>();
            DataRow[] dataRows = data.Select();
            foreach (var code in codes) {
                var rows = dataRows.Where(o => o[start] as string == code).ToArray();
                var result = MergeCells(0, rows, startRow, columnNames.Count, columnNames);
                merges = merges.Concat(result).ToList();
                startRow += rows.Count();
            }
            return merges;
        }

        private static List<MergeCell> MergeCells(int col, DataRow[] rows, int startMerge, int columnCount, List<string> columnNames) {
            char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            var merges = new List<MergeCell>();

            var distinctDatas = rows.Select(o => o[col].ToString()).Distinct().ToList();
            var startRow = startMerge + 1;
            foreach (var item in distinctDatas) {
                DataRow[] dataRows = rows.Where(o => o[col].ToString() == item).ToArray();
                var endRow = dataRows.Count() + startRow - 1;
                if (dataRows.Count() > 1) {
                    merges.Add(new MergeCell() { Reference = new StringValue($"{alphabet[col]}{startRow}:{alphabet[col]}{endRow}") });
                    if (col < columnCount - 1 && dataRows.Count() > 1) {
                        var result = MergeCells(col + 1, dataRows, startRow - 1, columnCount, columnNames);
                        merges = merges.Concat(result).ToList();
                    }
                }
                startRow = endRow + 1;
            }
            return merges;
        }

        public static Row[] GetBody(DataRowCollection data, int[] styles, List<string> columnNames) {
            var rows = new List<Row>();
            foreach (DataRow dsrow in data) {
                Row newRow = new Row() { };
                var columnIndex = 0;
                foreach (var col in columnNames) {
                    var style = styles[columnIndex];
                    Cell cell = new Cell {
                        CellValue = new CellValue(dsrow[col].ToString()),
                        DataType = style != 2 ? CellValues.Number : CellValues.String,
                        StyleIndex = (uint)style,
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
