using System.Collections.Generic;
using System.ComponentModel;

namespace DR.Export.RecieptPayment {

    public class RecieptPaymentData {
        public string From { get; set; }
        public string To { get; set; }
        public string TotalQuantity { get; set; }
        public List<RecieptPaymentItem> Items { get; set; } = new List<RecieptPaymentItem>();
    }

    public class RecieptPaymentItem {

        [Description("STT")]
        public int No { get; set; }

        [Description("Ngày giao dịch")]
        public string Date { get; set; }

        [Description("Loại phiếu")]
        public string Type { get; set; }

        [Description("Mã phiếu")]
        public string Code { get; set; }

        [Description("Khách hàng")]
        public string Customer { get; set; }

        [Description("Lý do")]
        public string Reason { get; set; }

        [Description("Thu tiền")]
        public string Value { get; set; }
    }
}
