using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DR.Export.ReportExport.ReportOrder {
    public class ReportOrderData {
        public string From { get; set; }
        public string To { get; set; }
        public decimal Total => Orders?.Sum(o => o.SubTotal) ?? 0;
        public decimal TotalBag => Orders?.Sum(o => o.Bag) ?? 0;
        public decimal TotalKg => Orders?.Sum(o => o.Kg) ?? 0;
        public List<ReportOrderDataItem> Orders { get; set; }
    }

    public class ReportOrderDataItem {

        [Description("STT")]
        public int No { get; set; }

        [Description("Mã khách hàng")]
        public string CustomerCode { get; set; }

        [Description("Tên khách hàng")]
        public string CustomerName { get; set; }

        [Description("Thành phố/Tỉnh")]
        public string Province { get; set; }

        [Description("Quận huyện")]
        public string District { get; set; }

        [Description("Phường/Xã")]
        public string Commune { get; set; }

        [Description("Mã sản phẩm")]
        public string ProductCode { get; set; }

        [Description("Tên sản phẩm")]
        public string ProductName { get; set; }

        [Description("Khối lượng\n(Kg)")]
        public string Weight { get; set; }

        [Description("Bao")]
        public decimal Bag { get; set; } = 0;

        [Description("Kg")]
        public decimal Kg { get; set; } = 0;

        [Description("Thành Tiền\n(VNĐ)")]
        public decimal SubTotal { get; set; }
    }
}
