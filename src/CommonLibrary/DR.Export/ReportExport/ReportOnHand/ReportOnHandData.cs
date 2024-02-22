using System.Collections.Generic;
using System.ComponentModel;

namespace DR.Export.ReportExport.ReportOnHand {

    public class ReportOnHandData {
        public string From { get; set; }
        public string To { get; set; }
        public List<ReportOnHandItem> Items { get; set; }
    }

    public class ReportOnHandItem {

        [Description("STT")]
        public int No { get; set; }

        [Description("Mã sản phẩm")]
        public string ProductCode { get; set; }

        [Description("Tên sản phẩm")]
        public string ProductName { get; set; }

        [Description("Thương hiệu")]
        public string BrandName { get; set; }

        [Description("Kho")]
        public string WarehouseName { get; set; }

        [Description("Đầu kỳ")]
        public decimal StockBefore { get; set; }

        [Description("Nhập trong kỳ")]
        public decimal StockImport { get; set; }

        [Description("Xuất trong kỳ")]
        public decimal StockExport { get; set; }

        [Description("Cuối kỳ")]
        public decimal StockAfter { get; set; }
    }
}
