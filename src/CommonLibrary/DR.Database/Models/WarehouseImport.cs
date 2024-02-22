using DR.Constant.Enums;

namespace DR.Database.Models;

public partial class WarehouseImport : IEntity {
    public string Id { get; set; } = null!;
    public string MerchantId { get; set; } = null!;

    [Description("Loại phiếu")]
    public EWarehouseImportType Type { get; set; } = EWarehouseImportType.Supplier;

    [Description("Nhà cung cấp")]
    public string? SupplierId { get; set; }

    [Description("Kho")]
    public string WarehouseId { get; set; } = null!;

    [Description("Trạng thái phiếu nhập")]
    public EWarehouseImport Status { get; set; }

    [Description("Mã phiếu nhập")]
    public string Code { get; set; } = null!;

    [Description("Ghi chú")]
    public string? Description { get; set; }

    [Description("Mã chứng từ nhà cung cấp")]
    public string? DeliveryCode { get; set; }

    [Description("Người giao hàng")]
    public string? DeliveryBy { get; set; }

    [Description("Ngày giao hàng")]
    public DateTimeOffset DeliveryDate { get; set; } = DateTimeOffset.UtcNow;

    public string CreatedBy { get; set; } = null!;

    [Description("Ngày tạo phiếu nhập")]
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

    public string? LastUpdateBy { get; set; }
    public DateTimeOffset LastUpdateDate { get; set; } = DateTimeOffset.UtcNow;

    [Description("Ngày nhận hàng")]
    public DateTimeOffset? ReceviedDate { get; set; }

    public decimal Total { get; set; }

    public virtual Supplier? Supplier { get; set; }
    public virtual Warehouse? Warehouse { get; set; }
    public virtual User? CreatedUser { get; set; }
    public virtual User? UpdatedUser { get; set; }
    public virtual ICollection<WarehouseImportDetail>? WarehouseImportDetails { get; set; }
}

internal class WarehouseImportConfig : IEntityTypeConfiguration<WarehouseImport> {

    public void Configure(EntityTypeBuilder<WarehouseImport> builder) {
        builder.ToTable(nameof(WarehouseImport));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32).IsRequired();

        builder.Property(o => o.MerchantId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.SupplierId).HasMaxLength(32);
        builder.Property(o => o.WarehouseId).HasMaxLength(32).IsRequired();

        builder.Property(o => o.CreatedBy).HasMaxLength(32).IsRequired();
        builder.Property(o => o.Code).HasMaxLength(50).IsRequired();
        builder.Property(o => o.Status).HasMaxLength(20).IsRequired();
        builder.Property(o => o.Description).HasMaxLength(2000);

        builder.Property(o => o.DeliveryDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o));
        builder.Property(o => o.CreatedDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o));
        builder.Property(o => o.LastUpdateDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o));
        builder.Property(o => o.ReceviedDate).HasConversion(o => o.HasValue ? o.Value.ToUnixTimeMilliseconds() : -1, o => o >= 0 ? DateTimeOffset.FromUnixTimeMilliseconds(o) : null);

        // index
        builder.HasIndex(o => o.MerchantId);
        builder.HasIndex(o => new { o.MerchantId, o.Type });
        builder.HasIndex(o => new { o.MerchantId, o.Code }).IsUnique();

        // fk
        builder.HasOne(o => o.Supplier).WithMany(o => o.WarehouseImports).HasForeignKey(o => o.SupplierId);
        builder.HasOne(o => o.Warehouse).WithMany(o => o.WarehouseImports).HasForeignKey(o => o.WarehouseId);
        builder.HasOne(o => o.CreatedUser).WithMany(o => o.WarehouseImportCreateds).HasForeignKey(o => o.CreatedBy);
        builder.HasOne(o => o.UpdatedUser).WithMany(o => o.WarehouseImportUpdateds).HasForeignKey(o => o.LastUpdateBy);

        builder.HasMany(o => o.WarehouseImportDetails).WithOne(o => o.WarehouseImport).HasForeignKey(o => o.WarehouseImportId);
    }
}
