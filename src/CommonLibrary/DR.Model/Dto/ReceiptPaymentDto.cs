using System.ComponentModel;

namespace DR.Models.Dto {

    public enum EReceiptPayment {

        [Description("Phiếu thu")]
        Receipt = 1,

        [Description("Phiếu chi")]
        Payment = 2,
    }

    public class ReceiptPaymentDto {
        public string Id { get; set; } = string.Empty;
        public EReceiptPayment Type { get; set; }
        public string Code { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string PaymentMethodName { get; set; } = string.Empty;
        public DateTimeOffset TransactedAt { get; set; }
        public decimal Value { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsDelete { get; set; }
    }
}
