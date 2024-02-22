using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DR.Export.ReportExport.ImportStock {

    public class ImportStockData {
        public string From { get; set; }
        public string To { get; set; }

        public decimal TotalQuantity => Items?.Sum(o => o.Quantity) ?? decimal.Zero;
        public decimal TotalWeight => Items?.Sum(o => o.Weight) ?? decimal.Zero;
        public List<ImportStockItem> Items { get; set; }
    }

    public class ImportStockItem {

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

        [Description("Người giao hàng")]
        public string Deliver { get; set; } = string.Empty;

        [Description("Nhà cung cấp")]
        public string SupplierName { get; set; }

        [Description("Tổng theo bao")]
        public decimal Quantity { get; set; }

        [Description("Tổng theo tấn")]
        public decimal Weight { get; set; }
    }
}
