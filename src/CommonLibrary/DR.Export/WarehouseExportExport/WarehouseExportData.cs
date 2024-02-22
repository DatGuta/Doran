using System.Collections.Generic;

namespace DR.Export.WarehouseExportExport {

    public class WarehouseExportData {
        public string day { get; set; }
        public string month { get; set; }
        public string year { get; set; }

        public string warehouseexport_no { get; set; }
        public string order_no { get; set; }
        public string customer_name { get; set; }
        public string customer_phone { get; set; }
        public string customer_address { get; set; }

        public string total_quantity { get; set; }
        public List<WarehouseExportDataItem> products { get; set; }
    }

    public class WarehouseExportDataItem {
        public string no { get; set; }
        public string product_name { get; set; }
        public string quantity { get; set; }
    }
}
