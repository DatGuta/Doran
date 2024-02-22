using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DR.Export.ReportExport.ReportProduct {

    public class ReportProductData {
        public string From { get; set; }
        public string To { get; set; }
        public decimal SumOrdered => Items?.Sum(o => o.Ordered) ?? 0;
        public decimal SumExported => Items?.Sum(o => o.Exported) ?? 0;
        public decimal SumRefunded => Items?.Sum(o => o.Refunded) ?? 0;
        public decimal Total => Items?.Sum(o => o.SubTotal) ?? 0;
        public List<ReportProductItem> Items { get; set; }
    }

    public class ReportProductItem {

        [Description("STT")]
        public int No { get; set; }

        [Description("Mã đơn hàng")]
        public string OrderNo { get; set; }

        [Description("Ngày tạo")]
        public string CreateAt { get; set; }

        [Description("Mã Khách hàng")]
        public string CustomerCode { get; set; }

        [Description("Tên Khách hàng")]
        public string CustomerName { get; set; }

        [Description("Thành phố/Tỉnh")]
        public string Province { get; set; }

        [Description("Quận/Huyện")]
        public string District { get; set; }

        [Description("Phường/xã")]
        public string Commune { get; set; }

        [Description("Mã Sản phẩm")]
        public string ProductCode { get; set; }

        [Description("Tên Sản phẩm")]
        public string ProductName { get; set; }

        [Description("Khối lượng\n(Kg)")]
        public decimal Weight { get; set; }

        [Description("Số lượng đặt\n(Bao)")]
        public decimal Ordered { get; set; }

        [Description("Số lượng xuất\n(Bao)")]
        public decimal Exported { get; set; }

        [Description("Số lượng trả\n(Bao)")]
        public decimal Refunded { get; set; }

        [Description("Đơn giá\n(VNĐ)")]
        public decimal Price { get; set; }

        [Description("Thành tiền\n(VNĐ)")]
        public decimal SubTotal { get; set; }
    }
}
