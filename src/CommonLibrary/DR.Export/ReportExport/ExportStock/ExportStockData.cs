using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DR.Export.ReportExport.ExportStock {

    public class ExportStockData {
        public string From { get; set; }
        public string To { get; set; }

        public decimal TotalQuantity => Items?.Sum(o => o.Quantity) ?? decimal.Zero;
        public decimal TotalWeight => Items?.Sum(o => o.Weight) ?? decimal.Zero;
        public List<ExportStockItem> Items { get; set; }
    }

    public class ExportStockItem {

        [Description("STT")]
        public int No { get; set; }

        [Description("Ngày phát sinh")]
        public string Date { get; set; }

        [Description("Mã sản phẩm")]
        public string ProductCode { get; set; } = string.Empty;

        [Description("Tên sản phẩm")]
        public string ProductName { get; set; } = string.Empty;

        [Description("Mã phiếu")]
        public string TransactionCode { get; set; } = string.Empty;

        [Description("Kho")]
        public string WarehouseName { get; set; } = string.Empty;

        [Description("Thương hiệu")]
        public string BrandName { get; set; }

        [Description("Tổng theo bao")]
        public decimal Quantity { get; set; }

        [Description("Tổng theo tấn")]
        public decimal Weight { get; set; }
    }
}
