using GemBox.Spreadsheet;
using System;
using System.IO;

namespace DR.Export {
    public abstract class BaseExporter {

        protected BaseExporter() {
            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
        }

        public abstract EExportFile GetExportFile();

        public abstract byte[] Export();

        public virtual byte[] ExportFile(string filename) {
            var exportFile = GetExportFile();
            switch (exportFile) {
                case EExportFile.Excel: return ExportFileExcel(filename);
                case EExportFile.Pdf: return ExportFilePdf(filename);
            }
            return Array.Empty<byte>();
        }

        private byte[] ExportFileExcel(string filename) {
            return File.ReadAllBytes(filename);
        }

        private byte[] ExportFilePdf(string filename) {
            var pdfFilename = $"temporary/{Path.GetFileNameWithoutExtension(filename)}{GetExportFile().FileExtension()}";
            try {
                var excel = ExcelFile.Load(filename);
                foreach (var ws in excel.Worksheets) {
                    ws.PrintOptions.PaperType = PaperType.A5;
                }
                excel.Save(pdfFilename, new PdfSaveOptions() { SelectionType = SelectionType.EntireFile });
                return File.ReadAllBytes(pdfFilename);
            } finally {
                if (File.Exists(pdfFilename))
                    File.Delete(pdfFilename);
            }
        }
    }
}
