using ClosedXML.Report;
using System;
using System.IO;

namespace DR.Export.OrderExport {
    public class OrderExporter : BaseExporter {
        private readonly OrderExportData data;
        private readonly string template;
        private readonly EExportFile exportFile;

        private OrderExporter(OrderExportData data, string template, EExportFile exportFile) : base() {
            this.data = data;
            this.template = template;
            this.exportFile = exportFile;
        }

        public static byte[] Export(OrderExportData data, string template, EExportFile exportFile) {
            return new OrderExporter(data, template, exportFile).Export();
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
