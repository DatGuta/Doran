using ClosedXML.Report;
using System;
using System.IO;

namespace DR.Export.WarehouseExportExport {

    public class WarehouseExportExporter : BaseExporter {
        private readonly WarehouseExportData data;
        private readonly string template;
        private readonly EExportFile exportFile;

        private WarehouseExportExporter(WarehouseExportData data, string template, EExportFile exportFile) : base() {
            this.data = data;
            this.template = template;
            this.exportFile = exportFile;
        }

        public static byte[] Export(WarehouseExportData data, string template, EExportFile exportFile) {
            return new WarehouseExportExporter(data, template, exportFile).Export();
        }

        public override byte[] Export() {
            if (!Directory.Exists("temporary")) Directory.CreateDirectory("temporary");
            string filename = $"temporary/{Guid.NewGuid()}.xlsx";

            try {
                var xlTemplate = new XLTemplate(template);
                xlTemplate.AddVariable(data);
                xlTemplate.Generate();
                xlTemplate.SaveAs(filename);

                return base.ExportFile(filename);
            } finally {
                if (File.Exists(filename))
                    File.Delete(filename);
            }
        }

        public override EExportFile GetExportFile() => exportFile;
    }
}
