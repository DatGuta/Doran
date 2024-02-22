using System.ComponentModel;

namespace DR.Export.RpCustomerHistory {

    public class RpCustomerHistoryData {
        public string From { get; set; }
        public string To { get; set; }
        public string CustomerName { get; set; }

        public List<RpCustomerHistoryItem> Items { get; set; } = new List<RpCustomerHistoryItem>();
    }

    public class RpCustomerHistoryItem {

        [Description("STT")]
        public int No { get; set; }

        [Description("Ngày chứng từ")]
        public string DocumentDate { get; set; }

        [Description("Số chứng từ")]
        public string DocumentCode { get; set; }

        [Description("Lý do")]
        public string Reason { get; set; }

        [Description("[1]Nợ")]
        public decimal DebtBefore { get; set; }

        [Description("[1]Có")]
        public decimal BalanceBefore { get; set; }

        [Description("[2]Nợ")]
        public decimal Debt { get; set; }

        [Description("[2]Có")]
        public decimal Balance { get; set; }

        [Description("[3]Nợ")]
        public decimal DebtAfter { get; set; }

        [Description("[3]Có")]
        public decimal BalanceAfter { get; set; }
    }
}
