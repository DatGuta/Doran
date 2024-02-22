using DocumentFormat.OpenXml.Drawing.Diagrams;
using System.Collections.Generic;
using System.ComponentModel;

namespace DR.Export.ReportExport.ReportOrderDetail {

    public class ReportOrderDetail {
        public string From { get; set; }
        public string To { get; set; }

        public List<ReportOrderDetailItem> Items { get; set; }
    }

    public class ReportOrderDetailItem {

        [Description("Cửa hàng")]
        public string StoreName { get; set; }

        [Description("Kho")]
        public string WarehouseName { get; set; }

        [Description("Ngày tạo đơn")]
        public string CreateAt { get; set; }

        [Description("Mã đơn hàng")]
        public string OrderNo { get; set; }

        [Description("Mã Khách hàng")]
        public string CustomerCode { get; set; }

        [Description("Tên Khách hàng")]
        public string CustomerName { get; set; }

        [Description("Mã sản phẩm")]
        public string ProductCode { get; set; }

        [Description("Tên sản phẩm")]
        public string ProductName { get; set; }

        [Description("Số lượng")]
        public decimal Quantity { get; set; }

        [Description("Đơn giá")]
        public decimal Price { get; set; }

        [Description("Thành tiền")]
        public decimal Total { get; set; }

        public int OrderIndex { get; set; }
    }
}
