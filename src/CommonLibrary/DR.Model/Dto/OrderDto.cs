namespace DR.Models.Dto {

    public class OrderDto {
        public string? Id { get; set; }
        public string? OrderNo { get; set; }
        public EOrderStatus Status { get; set; } = EOrderStatus.Draft;
        public decimal SubTotal { get; set; }
        public decimal BillDiscount { get; set; }
        public decimal SubDiscount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalBill { get; set; }
        public decimal TotalOverExport { get; set; }
        public decimal TotalRefundQuantity { get; set; }
        public decimal TotalRefund { get; set; }
        public int ExportTime { get; set; }
        public int RefundTime { get; set; }

        public EOrderPaymentStatus PaymentStatus { get; set; } = EOrderPaymentStatus.Unpaid;
        public decimal Remaining { get; set; }
        public decimal Paid { get; set; }
        public decimal Refunded { get; set; }

        public string? Description { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public List<OrderDetailDto>? Items { get; set; }

        public StoreDto? Store { get; set; }
        public WarehouseDto? Warehouse { get; set; }
        public CustomerDto? Customer { get; set; }
        public bool IsUseCustomerInfo { get; set; }
        public DeliveryDto? Delivery { get; set; }
        public DiscountDto? Discount { get; set; }
        public UserDto? CreatedBy { get; set; }

        public List<OrderActionDto> Actions { get; set; } = new();

        public List<ReceiptPaymentDto>? ReceiptPayments { get; set; }

        [return: NotNullIfNotNull(nameof(entity))]
        public static OrderDto? FromEntity(Database.Models.Order? entity, UnitResource? unitRes) {
            if (entity == null) return default;

            return new OrderDto {
                Id = entity.Id,
                OrderNo = entity.OrderNo,
                Status = entity.Status,
                SubTotal = entity.SubTotal,
                BillDiscount = entity.BillDiscount,
                SubDiscount = entity.SubDiscount,
                TotalDiscount = entity.TotalDiscount,
                TotalBill = entity.TotalBill,
                TotalOverExport = entity.TotalOverExport,
                TotalRefundQuantity = entity.TotalRefundQuantity,
                TotalRefund = entity.TotalRefund,
                ExportTime = entity.ExportTime,
                RefundTime = entity.RefundTime,
                PaymentStatus = entity.PaymentStatus,
                Remaining = entity.Remaining,
                Paid = entity.Paid,
                Refunded = entity.Refunded,
                IsUseCustomerInfo = entity.IsUseCustomerInfo,
                Description = entity.Description,
                CreatedDate = entity.CreatedDate,
                CreatedBy = UserDto.FromEntity(entity.User, unitRes),
                Customer = CustomerDto.FromEntity(entity.Customer, unitRes),
                Store = StoreDto.FromEntity(entity.Store, unitRes),
                Warehouse = WarehouseDto.FromEntity(entity.Warehouse, unitRes),
                Discount = new DiscountDto {
                    Id = entity.DiscountId,
                    Code = "",
                    Name = "",
                    Type = entity.DiscountType,
                    Value = entity.DiscountValue,
                },
                Items = entity.OrderDetails?.Select(o => OrderDetailDto.FromEntity(o)!).OrderBy(o => o.Product.Code).ToList(),
            };
        }
    }
}
