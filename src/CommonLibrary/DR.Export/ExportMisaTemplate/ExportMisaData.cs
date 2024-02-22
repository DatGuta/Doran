using System.Collections.Generic;
using System.ComponentModel;

namespace DR.Export.ExportMisaTemplate {

    public class ExportMisaData {
        public List<ExportMisaDataItem> Items { get; set; }
    }

    public class ExportMisaDataItem {

        [Description("Ngày hạch toán (*)")]
        public string AccountingDate { get; set; }

        [Description("Ngày chứng từ (*)")]
        public string DocumentDate { get; set; }

        [Description("Lý do xuất")]
        public string ReasonExport => !string.IsNullOrEmpty(CustomerName) ? "Xuất kho bán hàng " + CustomerName : "";

        [Description("Mã Khách hàng")]
        public string CustomerCode { get; set; }

        [Description("Tên Khách hàng")]
        public string CustomerName { get; set; }

        [Description("Diễn giải")]
        public string Explain => !string.IsNullOrEmpty(CustomerName) ? "Thu tiền bán hàng " + CustomerName : "";

        [Description("Mã hàng (*)")]
        public string ProductCode { get; set; }

        [Description("Tên hàng")]
        public string ProductName { get; set; }

        [Description("Số lượng")]
        public decimal Quantity { get; set; }

        [Description("Đơn giá sau thuế")]
        public decimal UnitPriceAfterTax { get; set; }

        [Description("Đơn giá")]
        public decimal Price { get; set; }

        [Description("Thành tiền")]
        public decimal Total { get; set; }
    }
}
