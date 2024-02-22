using System.Collections.Generic;

namespace DR.Export.OrderExport {
    public class OrderExportData {
        public string day { get; set; }
        public string month { get; set; }
        public string year { get; set; }

        public string order_no { get; set; }
        public string customer_name { get; set; }
        public string customer_address { get; set; }
        public string customer_phone { get; set; }

        public string total_quantity { get; set; }
        public string total { get; set; }
        public List<OrderExportDataItem> products { get; set; }
    }

    public class OrderExportDataItem {
        public string no { get; set; }
        public string product_name { get; set; }
        public string quantity { get; set; }
        public string price { get; set; }
        public string subtotal { get; set; }
    }
}
