namespace DR.Database.Models;

public partial class WarehouseExportOther {
    public string Id { get; set; } = null!;
    public string MerchantId { get; set; } = null!;

    [Description("Kho xuất")]
    public string WarehouseId { get; set; } = null!;

    [Description("Mã phiếu xuất")]
    public string Code { get; set; } = null!;

    [Description("Ngày xuất")]
    public DateTimeOffset ExportedDate { get; set; } = DateTimeOffset.Now;

    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;

    [Description("Ghi chú")]
    public string? Description { get; set; }

    public bool IsDelete { get; set; }

    public virtual Warehouse? Warehouse { get; set; }
    public virtual ICollection<WarehouseExportOtherDetail>? WarehouseExportOtherDetails { get; set; }
}

internal class WarehouseExportOtherConfig : IEntityTypeConfiguration<WarehouseExportOther> {

    public void Configure(EntityTypeBuilder<WarehouseExportOther> builder) {
        builder.ToTable(nameof(WarehouseExportOther));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32).IsRequired();

        builder.Property(o => o.MerchantId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.WarehouseId).HasMaxLength(32).IsRequired();

        builder.Property(o => o.Code).HasMaxLength(50).IsRequired();

        builder.Property(o => o.CreatedDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o));
        builder.Property(o => o.ExportedDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o));

        // index
        builder.HasIndex(o => o.MerchantId);
        builder.HasIndex(o => new { o.MerchantId, o.WarehouseId });
        builder.HasIndex(o => new { o.MerchantId, o.Code }).IsUnique();

        // fk
        builder.HasOne(o => o.Warehouse).WithMany(o => o.WarehouseExportOthers).HasForeignKey(o => o.WarehouseId);

        builder.HasMany(o => o.WarehouseExportOtherDetails).WithOne(o => o.WarehouseExportOther).HasForeignKey(o => o.WarehouseExportOtherId);
    }
}
