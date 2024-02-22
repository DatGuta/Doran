namespace DR.Models.Dto {

    public class PaymentDto {
        public string Id { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public CustomerDto? Customer { get; set; }
        public string? RecipientName { get; set; }
        public string? RecipientPhone { get; set; }
        public string? RecipientAddress { get; set; }
        public PaymentMethodDto? PaymentMethod { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public bool IsDelete { get; set; }
        public EPayment Type { get; set; }
        public List<PaymentItemDto> Items { get; set; } = new();
        public bool IsSelected { get; set; }
        public DateTimeOffset PaymentDate { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

        public static PaymentDto FromEntity(Database.Models.Payment entity, Customer? customer, PaymentMethod? paymentMethod, List<PaymentDetail> paymentItems, UnitResource? unitRes) {
            return new PaymentDto {
                Id = entity.Id,
                Code = entity.Code,
                CreatedDate = entity.CreatedDate,
                PaymentDate = entity.PaymentDate,
                Customer = CustomerDto.FromEntity(customer, unitRes),
                RecipientName = entity.RecipientName,
                RecipientPhone = entity.RecipientPhone,
                RecipientAddress = entity.RecipientAddress,
                PaymentMethod = PaymentMethodDto.FromEntity(paymentMethod),
                Description = entity.Description,
                Value = entity.Value,
                Type = entity.Type,
                IsDelete = entity.IsDelete,
                IsSelected = false,
                Items = paymentItems.Where(x => x.Order != null).Select(o => PaymentItemDto.FormEntity(o, o.Order!)).ToList(),
            };
        }

        public Payment ToEntity(string merchantId, string userId) {
            return new Payment {
                Id = NGuidHelper.New(Id),
                MerchantId = merchantId,
                CreatedBy = userId,
                Code = Code,
                CreatedDate = CreatedDate,
                PaymentDate = PaymentDate,
                CustomerId = Customer!.Id!,
                RecipientName = RecipientName,
                RecipientPhone = RecipientPhone,
                RecipientAddress = RecipientAddress,
                PaymentMethodId = PaymentMethod!.Id!,
                Description = Description,
                Value = Value,
                Type = Type,
                IsDelete = false,
                PaymentDetails = Items?.Where(o => o.IsSelected)
                    .Select(o => new PaymentDetail {
                        Id = NGuidHelper.New(),
                        OrderId = o.Id,
                        Value = o.Value,
                    }).ToList()
            };
        }
    }
}
