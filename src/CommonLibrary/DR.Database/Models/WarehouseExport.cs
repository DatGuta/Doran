using DR.Constant.Enums;

namespace DR.Database.Models;

public partial class WarehouseExport : IEntity {
    public string Id { get; set; } = null!;
    public string MerchantId { get; set; } = null!;

    [Description("Kho xuất")]
    public string WarehouseId { get; set; } = null!;

    [Description("Từ đơn hàng")]
    public string? OrderId { get; set; }

    [Description("Mã phiếu xuất")]
    public string Code { get; set; } = null!;

    [Description("Trạng thái")]
    public EWarehouseExport Status { get; set; }

    public string? CustomerId { get; set; }

    [Description("Đến khách hàng")]
    public string? CustomerName { get; set; }

    public string? CustomerPhone { get; set; }
    public string? CustomerAddress { get; set; }

    public string CreatedBy { get; set; } = null!;

    [Description("Ngày tạo")]
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

    public string? LastUpdateBy { get; set; }
    public DateTimeOffset LastUpdateDate { get; set; } = DateTimeOffset.UtcNow;

    [Description("Ngày xuất")]
    public DateTimeOffset? ExportedDate { get; set; }

    [Description("Ghi chú")]
    public string? Description { get; set; }

    public virtual Warehouse? Warehouse { get; set; }
    public virtual Order? Order { get; set; }
    public virtual Customer? Customer { get; set; }
    public virtual User? CreatedUser { get; set; }
    public virtual User? UpdatedUser { get; set; }
    public virtual ICollection<WarehouseRefund>? WarehouseRefunds { get; set; }
    public virtual ICollection<WarehouseExportDetail>? WarehouseExportDetails { get; set; }
}

internal class WarehouseExportConfig : IEntityTypeConfiguration<WarehouseExport> {

    public void Configure(EntityTypeBuilder<WarehouseExport> builder) {
        builder.ToTable(nameof(WarehouseExport));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32).IsRequired();

        builder.Property(o => o.MerchantId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.WarehouseId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.OrderId).HasMaxLength(32);
        builder.Property(o => o.CustomerId).HasMaxLength(32);

        builder.Property(o => o.CreatedBy).HasMaxLength(32).IsRequired();
        builder.Property(o => o.Code).HasMaxLength(50).IsRequired();
        builder.Property(o => o.Status).HasMaxLength(20).IsRequired();

        builder.Property(o => o.CreatedDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o));
        builder.Property(o => o.LastUpdateDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o));
        builder.Property(o => o.ExportedDate).HasConversion(o => o.HasValue ? o.Value.ToUnixTimeMilliseconds() : -1, o => o >= 0 ? DateTimeOffset.FromUnixTimeMilliseconds(o) : null);

        // index
        builder.HasIndex(o => o.MerchantId);
        builder.HasIndex(o => new { o.MerchantId, o.Code }).IsUnique();

        // fk
        builder.HasOne(o => o.Warehouse).WithMany(o => o.WarehouseExports).HasForeignKey(o => o.WarehouseId);
        builder.HasOne(o => o.Order).WithMany(o => o.WarehouseExports).HasForeignKey(o => o.OrderId);
        builder.HasOne(o => o.Customer).WithMany(o => o.WarehouseExports).HasForeignKey(o => o.CustomerId);
        builder.HasOne(o => o.CreatedUser).WithMany(o => o.WarehouseExportCreateds).HasForeignKey(o => o.CreatedBy);
        builder.HasOne(o => o.UpdatedUser).WithMany(o => o.WarehouseExportUpdateds).HasForeignKey(o => o.LastUpdateBy);

        builder.HasMany(o => o.WarehouseRefunds).WithOne(o => o.WarehouseExport).HasForeignKey(o => o.WarehouseExportId);
        builder.HasMany(o => o.WarehouseExportDetails).WithOne(o => o.WarehouseExport).HasForeignKey(o => o.WarehouseExportId);
    }
}
