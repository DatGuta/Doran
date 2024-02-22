namespace DR.Contexts.NormalContexts {

    public class UpdateCustomerTrackingContext(string merchantId, string customerId) {
        public string MerchantId { get; } = merchantId;
        public string CustomerId { get; } = customerId;
    }
}
