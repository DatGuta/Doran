namespace DR.Contexts.NormalContexts {

    public class UpdateNoOfCusByCusGrpContext(string merchantId, HashSet<string> customerGroupIds) {
        public string MerchantId { get; } = merchantId;
        public HashSet<string> CustomerGroupIds { get; } = customerGroupIds;
    }
}
