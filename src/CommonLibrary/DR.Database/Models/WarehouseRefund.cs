using DR.Constant.Enums;

namespace DR.Database.Models;

public partial class WarehouseRefund : IEntity {
    public string Id { get; set; } = null!;
    public string MerchantId { get; set; } = null!;

    [Description("Kho nhập lại")]
    public string WarehouseId { get; set; } = null!;

    [Description("Mã đơn hàng")]
    public string? OrderId { get; set; }

    [Description("Mã phiếu xuất")]
    public string WarehouseExportId { get; set; } = null!;

    [Description("Trạng thái")]
    public EWarehouseRefund Status { get; set; }

    [Description("Mã phiếu trả hàng")]
    public string Code { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    [Description("Ngày tạo")]
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

    public string? LastUpdateBy { get; set; }
    public DateTimeOffset LastUpdateDate { get; set; } = DateTimeOffset.UtcNow;

    [Description("Ngày trả hàng")]
    public DateTimeOffset? RefundDate { get; set; }

    [Description("Lý do trả hàng")]
    public string? Description { get; set; }

    public virtual Warehouse? Warehouse { get; set; }
    public virtual Order? Order { get; set; }
    public virtual WarehouseExport? WarehouseExport { get; set; }
    public virtual User? CreatedUser { get; set; }
    public virtual User? UpdatedUser { get; set; }
    public virtual ICollection<WarehouseRefundDetail>? WarehouseRefundDetails { get; set; }
}

internal class WarehouseRefundConfig : IEntityTypeConfiguration<WarehouseRefund> {

    public void Configure(EntityTypeBuilder<WarehouseRefund> builder) {
        builder.ToTable(nameof(WarehouseRefund));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32).IsRequired();

        builder.Property(o => o.MerchantId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.WarehouseId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.OrderId).HasMaxLength(32);
        builder.Property(o => o.WarehouseExportId).HasMaxLength(32);

        builder.Property(o => o.CreatedBy).HasMaxLength(32).IsRequired();
        builder.Property(o => o.LastUpdateBy).HasMaxLength(32);
        builder.Property(o => o.Code).HasMaxLength(50).IsRequired();
        builder.Property(o => o.Status).HasMaxLength(20).IsRequired();

        builder.Property(o => o.CreatedDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o));
        builder.Property(o => o.LastUpdateDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o));
        builder.Property(o => o.RefundDate).HasConversion(o => o.HasValue ? o.Value.ToUnixTimeMilliseconds() : -1, o => o >= 0 ? DateTimeOffset.FromUnixTimeMilliseconds(o) : null);

        // index
        builder.HasIndex(o => o.MerchantId);
        builder.HasIndex(o => new { o.MerchantId, o.Code }).IsUnique();

        // fk
        builder.HasOne(o => o.Warehouse).WithMany(o => o.WarehouseRefunds).HasForeignKey(o => o.WarehouseId);
        builder.HasOne(o => o.Order).WithMany(o => o.WarehouseRefunds).HasForeignKey(o => o.OrderId);
        builder.HasOne(o => o.CreatedUser).WithMany(o => o.WarehouseRefundCreateds).HasForeignKey(o => o.CreatedBy);
        builder.HasOne(o => o.UpdatedUser).WithMany(o => o.WarehouseRefundUpdateds).HasForeignKey(o => o.LastUpdateBy);
        builder.HasOne(o => o.WarehouseExport).WithMany(o => o.WarehouseRefunds).HasForeignKey(o => o.WarehouseExportId);

        builder.HasMany(o => o.WarehouseRefundDetails).WithOne(o => o.WarehouseRefund).HasForeignKey(o => o.WarehouseRefundId);
    }
}
