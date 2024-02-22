using DR.Constant.Enums;

namespace DR.Database.Models;

public partial class OrderDetail {
    public string Id { get; set; } = null!;
    public string OrderId { get; set; } = null!;
    public string? ProductId { get; set; }
    public string GroupId { get; set; } = null!;
    public bool IsOverThreshold { get; set; }

    public int OrderIndex { get; set; }

    [Description("Mã sản phẩm")]
    public string? ProductCode { get; set; }

    [Description("Tên sản phẩm")]
    public string? ProductName { get; set; }

    [Description("Tên hiển thị")]
    public string? ProductDisplay { get; set; }

    public bool IsPromotion { get; set; }
    public string? DiscountId { get; set; }
    public EDiscount DiscountType { get; set; } = EDiscount.None;
    public decimal DiscountValue { get; set; }

    [Description("Đơn giá")]
    public decimal Price { get; set; }

    [Description("Số lượng")]
    public decimal Quantity { get; set; }

    public decimal SubTotal { get; set; }
    public decimal ItemDiscount { get; set; }

    [Description("Thành tiền")]
    public decimal TotalItem { get; set; }

    public decimal ExportQuantity { get; set; }
    public decimal RefundQuantity { get; set; }
    public decimal RefundAmount { get; set; }

    public virtual Order? Order { get; set; }
    public virtual Product? Product { get; set; }

    public virtual ICollection<OrderActionDetail>? OrderActionDetails { get; set; }
}

internal class OrderDetailConfig : IEntityTypeConfiguration<OrderDetail> {

    public void Configure(EntityTypeBuilder<OrderDetail> builder) {
        builder.ToTable(nameof(OrderDetail));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);

        builder.Property(o => o.ProductName).HasMaxLength(255);
        builder.Property(o => o.ProductDisplay).HasMaxLength(255);

        builder.Property(o => o.OrderId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.ProductId).HasMaxLength(32);
        builder.Property(o => o.GroupId).HasMaxLength(32);

        builder.Property(o => o.DiscountId).HasMaxLength(32);

        // index
        builder.HasIndex(o => o.OrderId);

        // fk
        builder.HasOne(o => o.Order).WithMany(o => o.OrderDetails).HasForeignKey(o => o.OrderId);
        builder.HasOne(o => o.Product).WithMany(o => o.OrderDetails).HasForeignKey(o => o.ProductId);

        builder.HasMany(o => o.OrderActionDetails).WithOne(o => o.OrderDetail).HasForeignKey(o => o.OrderDetailId);
    }
}
