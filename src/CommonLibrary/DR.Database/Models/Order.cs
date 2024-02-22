using DR.Constant.Enums;

namespace DR.Database.Models;

public partial class Order : IEntity {
    public string Id { get; set; } = null!;
    public string MerchantId { get; set; } = null!;

    [Description("Cửa hàng")]
    public string StoreId { get; set; } = null!;

    [Description("Kho hàng")]
    public string? WarehouseId { get; set; }

    [Description("Loại đơn hàng")]
    public EOrder Type { get; set; }

    [Description("Mã đơn hàng")]
    public string OrderNo { get; set; } = null!;

    [Description("Trạng thái đơn hàng")]
    public EOrderStatus Status { get; set; } = EOrderStatus.Draft;

    [Description("Khách hàng")]
    public string? CustomerId { get; set; }

    public string? DiscountId { get; set; }
    public EDiscount DiscountType { get; set; } = EDiscount.None;
    public decimal DiscountValue { get; set; }
    public decimal SubTotal { get; set; }
    public decimal BillDiscount { get; set; }
    public decimal SubDiscount { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal TotalOverExport { get; set; }
    public decimal TotalRefundQuantity { get; set; }
    public decimal TotalRefund { get; set; }
    public int ExportTime { get; set; }
    public int RefundTime { get; set; }
    public int PrintNumber { get; set; }

    [Description("Tổng đơn")]
    public decimal TotalBill { get; set; }

    [Description("Trạng thái đơn hàng")]
    public EOrderPaymentStatus PaymentStatus { get; set; } = EOrderPaymentStatus.Unpaid;

    [Description("Số tiền nợ")]
    public decimal Remaining { get; set; }

    [Description("Số tiền đã thanh toán")]
    public decimal Paid { get; set; }

    public decimal Refunded { get; set; }

    [Description("Ghi chú")]
    public string? Description { get; set; }

    public bool IsUseCustomerInfo { get; set; }

    [Description("Ngày tạo")]
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset ModifiedDate { get; set; } = DateTimeOffset.UtcNow;

    public string? CreatedBy { get; set; }

    public virtual Store? Store { get; set; }
    public virtual Warehouse? Warehouse { get; set; }
    public virtual Customer? Customer { get; set; }
    public virtual User? User { get; set; }

    [Description("Danh sách sản phẩm")]
    public virtual ICollection<OrderDetail>? OrderDetails { get; set; }

    public virtual ICollection<OrderAction>? OrderActions { get; set; }

    public virtual ICollection<OrderCustomer>? OrderCustomers { get; set; }
    public virtual ICollection<OrderPayment>? OrderPayments { get; set; }
    public virtual ICollection<WarehouseExport>? WarehouseExports { get; set; }
    public virtual ICollection<WarehouseRefund>? WarehouseRefunds { get; set; }
    public virtual ICollection<ReceiptDetail>? ReceiptDetails { get; set; }
    public virtual ICollection<PaymentDetail>? PaymentDetails { get; set; }
}

internal class OrderConfig : IEntityTypeConfiguration<Order> {

    public void Configure(EntityTypeBuilder<Order> builder) {
        builder.ToTable(nameof(Order));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);
        builder.Property(o => o.MerchantId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.StoreId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.WarehouseId).HasMaxLength(32);

        builder.Property(o => o.CustomerId).HasMaxLength(32);
        builder.Property(o => o.DiscountId).HasMaxLength(32);

        builder.Property(o => o.OrderNo).HasMaxLength(50).IsRequired();

        builder.Property(o => o.Description).HasMaxLength(2000);

        builder.Property(o => o.CreatedDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();
        builder.Property(o => o.ModifiedDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();
        builder.Property(o => o.CreatedBy).HasMaxLength(32);

        // index
        builder.HasIndex(o => o.MerchantId);
        builder.HasIndex(o => new { o.MerchantId, o.OrderNo }).IsUnique();
        builder.HasIndex(o => new { o.MerchantId, o.StoreId, o.OrderNo }).IsUnique();

        // fk
        builder.HasOne(o => o.Store).WithMany(o => o.Orders).HasForeignKey(o => o.StoreId);
        builder.HasOne(o => o.Warehouse).WithMany(o => o.Orders).HasForeignKey(o => o.WarehouseId);
        builder.HasOne(o => o.Customer).WithMany(o => o.Orders).HasForeignKey(o => o.CustomerId);
        builder.HasOne(o => o.User).WithMany(o => o.Orders).HasForeignKey(o => o.CreatedBy);

        builder.HasMany(o => o.OrderDetails).WithOne(o => o.Order).HasForeignKey(o => o.OrderId);
        builder.HasMany(o => o.OrderActions).WithOne(o => o.Order).HasForeignKey(o => o.OrderId);
        builder.HasMany(o => o.OrderCustomers).WithOne(o => o.Order).HasForeignKey(o => o.OrderId);
        builder.HasMany(o => o.WarehouseExports).WithOne(o => o.Order).HasForeignKey(o => o.OrderId);
        builder.HasMany(o => o.WarehouseRefunds).WithOne(o => o.Order).HasForeignKey(o => o.OrderId);
        builder.HasMany(o => o.ReceiptDetails).WithOne(o => o.Order).HasForeignKey(o => o.OrderId);
        builder.HasMany(o => o.PaymentDetails).WithOne(o => o.Order).HasForeignKey(o => o.OrderId);
    }
}
