namespace DR.Database.Models;

public partial class WarehouseExportDetail {
    public string Id { get; set; } = null!;
    public string WarehouseExportId { get; set; } = null!;
    public string ProductId { get; set; } = null!;
    public string? OrderDetailId { get; set; }
    public decimal Quantity { get; set; }

    public virtual WarehouseExport? WarehouseExport { get; set; }
    public virtual Product? Product { get; set; }

    public virtual ICollection<WarehouseRefundDetail>? WarehouseRefundDetails { get; set; }
}

internal class WarehouseExportDetailConfig : IEntityTypeConfiguration<WarehouseExportDetail> {

    public void Configure(EntityTypeBuilder<WarehouseExportDetail> builder) {
        builder.ToTable(nameof(WarehouseExportDetail));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32).IsRequired();

        builder.Property(o => o.WarehouseExportId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.ProductId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.OrderDetailId).HasMaxLength(32);

        // index
        builder.HasIndex(o => o.WarehouseExportId);

        // fk
        builder.HasOne(o => o.WarehouseExport).WithMany(o => o.WarehouseExportDetails).HasForeignKey(o => o.WarehouseExportId);
        builder.HasOne(o => o.Product).WithMany(o => o.WarehouseExportDetails).HasForeignKey(o => o.ProductId);

        builder.HasMany(o => o.WarehouseRefundDetails).WithOne(o => o.WarehouseExportDetail).HasForeignKey(o => o.WarehouseExportDetailId);
    }
}
