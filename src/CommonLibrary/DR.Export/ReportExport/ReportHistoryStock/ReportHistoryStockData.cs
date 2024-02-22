using System.Collections.Generic;
using System.ComponentModel;

namespace DR.Export.ReportExport.ReportHistoryStock {
    public class ReportHistoryStockData {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public List<ReportHistoryStockItem> Items { get; set; }
    }

    public class ReportHistoryStockItem {

        [Description("Ngày phát sinh")]
        public string Date { get; set; }

        [Description("Giao dịch")]
        public string TransactionType { get; set; }

        [Description("Mã phiếu")]
        public string TransactionCode { get; set; }

        [Description("Tên KH/NCC")]
        public string TargetName { get; set; }

        [Description("Đầu kỳ")]
        public string OnHandBefore { get; set; }

        [Description("Nhập trong kỳ")]
        public string ImportQuantity { get; set; }

        [Description("Xuất trong kỳ")]
        public string ExportQuantity { get; set; }

        [Description("Cuối kỳ")]
        public string OnHandAfter { get; set; }
    }
}
