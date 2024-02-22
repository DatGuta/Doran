namespace DR.Models.Dto {

    public class ReceiptDto {
        public string Id { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset ReceiptDate { get; set; }
        public CustomerDto? Customer { get; set; }
        public PaymentMethodDto? PaymentMethod { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public bool IsDelete { get; set; }
        public List<ReceiptItemDto> Items { get; set; } = new();

        public static ReceiptDto FromEntity(Receipt entity, Customer? customer, PaymentMethod? paymentMethod,
            List<ReceiptDetail> items, List<OrderCustomerDebtDto> debts, UnitResource? unitRes) {
            return new ReceiptDto {
                Id = entity.Id,
                Code = entity.Code,
                CreatedDate = entity.CreatedDate,
                ReceiptDate = entity.ReceiptDate,
                Customer = CustomerDto.FromEntity(customer, unitRes),
                PaymentMethod = PaymentMethodDto.FromEntity(paymentMethod),
                Description = entity.Description,
                Value = entity.Value,
                IsDelete = entity.IsDelete,
                Items = debts.Select(o => {
                    var item = items.Find(x => x.OrderId == o.Id);
                    return new ReceiptItemDto {
                        OrderId = o.Id,
                        OrderNo = o.OrderNo,
                        OriginRemaining = o.Remaining + (item?.Value ?? decimal.Zero),
                        Remaining = o.Remaining,
                        TotalBill = o.TotalBill,
                        Value = item?.Value ?? decimal.Zero,
                        CreatedDate = o.CreatedDate,
                        HasWarehouseExport = o.HasWarehouseExport,
                        IsSelected = item != null,
                    };
                }).OrderBy(o => o.CreatedDate).ToList(),
            };
        }

        public Receipt ToEntity(string merchantId, string userId) {
            return new Receipt {
                Id = NGuidHelper.New(Id),
                MerchantId = merchantId,
                Code = Code,
                CustomerId = Customer!.Id!,
                PaymentMethodId = PaymentMethod!.Id!,
                Description = Description,
                Value = Value,
                CreatedDate = DateTimeOffset.UtcNow,
                ModifiedDate = DateTimeOffset.UtcNow,
                CreatedBy = userId,
                IsDelete = false,
                ReceiptDate = ReceiptDate,
                ReceiptDetails = Items?.Where(o => o.IsSelected)
                    .Select(o => new ReceiptDetail {
                        Id = NGuidHelper.New(),
                        OrderId = o.OrderId,
                        Value = o.Value,
                    }).ToList(),
            };
        }
    }
}
