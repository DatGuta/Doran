using System.Collections.Generic;
using System.ComponentModel;

namespace DR.Export.DebtHistory {

    public class DebtHistoryReport {
        public string From { get; set; }
        public string To { get; set; }
        public List<DebtHistoryItem> Items { get; set; } = new List<DebtHistoryItem>();
    }

    public class DebtHistoryItem {

        [Description("STT")]
        public int No { get; set; }

        [Description("Ngày")]
        public string TransactedAt { get; set; }

        [Description("Đơn hàng")]
        public string OrderNo { get; set; }

        [Description("Phiếu")]
        public string Code { get; set; }

        [Description("Loại")]
        public string Type { get; set; }

        [Description("Thu")]
        public string Reciept { get; set; }

        [Description("Chi")]
        public string Payment { get; set; }

        [Description("Tổng tiền")]
        public string Total { get; set; }

        [Description("Nọ")]
        public string Debt { get; set; }

        [Description("Có")]
        public string Balance { get; set; }

        [Description("Lý do")]
        public string Reason { get; set; }
    }
}
