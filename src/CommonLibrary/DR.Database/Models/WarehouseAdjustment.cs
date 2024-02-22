namespace DR.Database.Models;

public partial class WarehouseAdjustment : IEntity {
    public string Id { get; set; } = null!;
    public string MerchantId { get; set; } = null!;

    [Description("Kho hàng")]
    public string WarehouseId { get; set; } = null!;

    [Description("Mã phiếu kiểm kho")]
    public string Code { get; set; } = null!;

    [Description("Ghi chú")]
    public string? Description { get; set; }

    public string CreatedBy { get; set; } = null!;

    [Description("Ngày tạo phiếu kiểm")]
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

    [Description("Người kiểm")]
    public string? Reporter { get; set; }

    [Description("Thời gian kiểm")]
    public DateTimeOffset ReportAt { get; set; } = DateTimeOffset.UtcNow;

    public bool IsAdjust { get; set; }
    public bool IsDelete { get; set; }

    public virtual Warehouse? Warehouse { get; set; }
    public virtual User? User { get; set; }
    public virtual ICollection<WarehouseAdjustmentDetail>? WarehouseAdjustmentDetails { get; set; }
}

internal class WarehouseAdjustmentConfig : IEntityTypeConfiguration<WarehouseAdjustment> {

    public void Configure(EntityTypeBuilder<WarehouseAdjustment> builder) {
        builder.ToTable(nameof(WarehouseAdjustment));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32).IsRequired();

        builder.Property(o => o.MerchantId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.WarehouseId).HasMaxLength(32).IsRequired();

        builder.Property(o => o.Code).HasMaxLength(50).IsRequired();

        builder.Property(o => o.CreatedBy).HasMaxLength(32).IsRequired();
        builder.Property(o => o.CreatedDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o));
        builder.Property(o => o.ReportAt).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o));

        // index
        builder.HasIndex(o => o.MerchantId);
        builder.HasIndex(o => new { o.MerchantId, o.Code }).IsUnique();

        // fk
        builder.HasOne(o => o.Warehouse).WithMany(o => o.WarehouseAdjustments).HasForeignKey(o => o.WarehouseId);
        builder.HasOne(o => o.User).WithMany(o => o.WarehouseAdjustments).HasForeignKey(o => o.CreatedBy);

        builder.HasMany(o => o.WarehouseAdjustmentDetails).WithOne(o => o.WarehouseAdjustment).HasForeignKey(o => o.WarehouseAdjustmentId);
    }
}
