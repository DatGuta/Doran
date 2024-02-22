using System.ComponentModel;

namespace DR.Export.ProductExporter {
    public class ProductExportData {

        [Description("STT")]
        public int No { get; set; }

        [Description("Mã sản phẩm")]
        public string Code { get; set; }

        [Description("Tên sản phẩm")]
        public string Name { get; set; }

        [Description("Tên hiển thị")]
        public string DisplayName { get; set; }

        [Description("Thương hiệu")]
        public string Brand { get; set; }

        [Description("NPk")]
        public string NPKType { get; set; }

        [Description("Khối lượng")]
        public decimal NetWeight { get; set; }
    }
}
