using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DR.Export.ReportExport.ReportCategory {

    public class ReportCategoryData {
        public string From { get; set; }
        public string To { get; set; }
        public decimal TotalOnHand => Items?.Sum(o => o.OnHand) ?? 0;
        public decimal TotalOrdered => Items?.Sum(o => o.Ordered) ?? 0;
        public decimal TotalRefund => Items?.Sum(o => o.Refund) ?? 0;
        public decimal Total => Items?.Sum(o => o.SubTotal) ?? 0;
        public List<ReportCategoryItem> Items { get; set; }
    }

    public class ReportCategoryItem {

        [Description("STT")]
        public int No { get; set; }

        [Description("Phân loại")]
        public string CategoryName { get; set; }

        [Description("Mã sản phẩm")]
        public string ProductCode { get; set; }

        [Description("Tên sản phẩm")]
        public string ProductName { get; set; }

        [Description("Tồn kho")]
        public decimal OnHand { get; set; }

        [Description("Số lượng bán")]
        public decimal Ordered { get; set; } = 0;

        [Description("Số lượng trả")]
        public decimal Refund { get; set; } = 0;

        [Description("Doanh số")]
        public decimal SubTotal { get; set; }
    }
}
