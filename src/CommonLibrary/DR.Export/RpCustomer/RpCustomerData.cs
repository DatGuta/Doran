using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DR.Export.RpCustomer {

    public class RpCustomerExport {
        public DateTimeOffset? From { get; set; }
        public DateTimeOffset? To { get; set; }
        public List<RpCustomerItem> Items { get; set; } = new List<RpCustomerItem>();
    }

    public class RpCustomerItem {

        [Description("STT")]
        public int No { get; set; }

        [Description("Mã khách hàng")]
        public string Code { get; set; }

        [Description("Tên khách hàng")]
        public string Name { get; set; }

        [Description("[1]Nợ")]
        public decimal OpeningDebtDisplay { get; set; }

        [Description("[1]Có")]
        public decimal OpeningBalanceDisplay { get; set; }

        [Description("[2]Nợ")]
        public decimal CurrentDebtDisplay { get; set; }

        [Description("[2]Có")]
        public decimal CurrentBalanceDisplay { get; set; }

        [Description("Nợ [Phải thu]")]
        public decimal EndDebtDisplay { get; set; }

        [Description("Có [Phải trả]")]
        public decimal EndBalanceDisplay { get; set; }
    }
}
