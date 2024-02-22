using DR.Constant.Enums;

namespace DR.Database.Models;

public partial class WarehouseTransfer : IEntity {
    public string Id { get; set; } = null!;
    public string MerchantId { get; set; } = null!;

    [Description("Mã phiếu chuyển kho")]
    public string Code { get; set; } = null!;

    [Description("Kho xuất")]
    public string FromWarehouseId { get; set; } = null!;

    [Description("Kho nhập")]
    public string ToWarehouseId { get; set; } = null!;

    [Description("Trạng thái đơn")]
    public EWarehouseTransfer Status { get; set; }

    [Description("Ngày xuất kho")]
    public DateTimeOffset? ExportedDate { get; set; }

    [Description("Ngày nhập kho")]
    public DateTimeOffset? ImportedDate { get; set; }

    [Description("Ghi chú")]
    public string? Description { get; set; }

    [Description("Tạo bởi")]
    public string CreatedBy { get; set; } = null!;

    [Description("Ngày tạo")]
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

    public string? LastUpdateBy { get; set; }
    public DateTimeOffset LastUpdateDate { get; set; } = DateTimeOffset.UtcNow;

    public virtual Warehouse? FromWarehouse { get; set; }
    public virtual Warehouse? ToWarehouse { get; set; }
    public virtual User? CreatedUser { get; set; }
    public virtual User? UpdatedUser { get; set; }
    public virtual ICollection<WarehouseTransferDetail>? WarehouseTransferDetails { get; set; }
}

internal class WarehouseTransferConfig : IEntityTypeConfiguration<WarehouseTransfer> {

    public void Configure(EntityTypeBuilder<WarehouseTransfer> builder) {
        builder.ToTable(nameof(WarehouseTransfer));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32).IsRequired();

        builder.Property(o => o.MerchantId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.Code).HasMaxLength(50).IsRequired();
        builder.Property(o => o.FromWarehouseId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.ToWarehouseId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.Description).HasMaxLength(2000);

        builder.Property(o => o.Status).HasMaxLength(20).IsRequired();
        builder.Property(o => o.ImportedDate).HasConversion(o => o.HasValue ? o.Value.ToUnixTimeMilliseconds() : -1, o => o >= 0 ? DateTimeOffset.FromUnixTimeMilliseconds(o) : null);
        builder.Property(o => o.ExportedDate).HasConversion(o => o.HasValue ? o.Value.ToUnixTimeMilliseconds() : -1, o => o >= 0 ? DateTimeOffset.FromUnixTimeMilliseconds(o) : null);

        builder.Property(o => o.CreatedBy).HasMaxLength(32).IsRequired();
        builder.Property(o => o.CreatedDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o));

        builder.Property(o => o.LastUpdateBy).HasMaxLength(32).IsRequired();
        builder.Property(o => o.LastUpdateDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o));

        // index
        builder.HasIndex(o => o.MerchantId);
        builder.HasIndex(o => new { o.MerchantId, o.Code }).IsUnique();

        // fk
        builder.HasOne(o => o.FromWarehouse).WithMany(o => o.FromWarehouseTransfers).HasForeignKey(o => o.FromWarehouseId);
        builder.HasOne(o => o.ToWarehouse).WithMany(o => o.ToWarehouseTransfers).HasForeignKey(o => o.ToWarehouseId);
        builder.HasOne(o => o.CreatedUser).WithMany(o => o.WarehouseTransferCreateds).HasForeignKey(o => o.CreatedBy);
        builder.HasOne(o => o.UpdatedUser).WithMany(o => o.WarehouseTransferUpdateds).HasForeignKey(o => o.LastUpdateBy);

        builder.HasMany(o => o.WarehouseTransferDetails).WithOne(o => o.WarehouseTransfer).HasForeignKey(o => o.WarehouseTransferId);
    }
}
