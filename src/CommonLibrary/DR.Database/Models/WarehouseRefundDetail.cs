namespace DR.Database.Models;
public partial class WarehouseRefundDetail {
    public string Id { get; set; } = null!;
    public string WarehouseRefundId { get; set; } = null!;
    public string? WarehouseExportDetailId { get; set; }
    public string ProductId { get; set; } = null!;

    public decimal ExportedQuantity { get; set; }
    public decimal RefundQuantity { get; set; }

    public virtual WarehouseRefund? WarehouseRefund { get; set; }
    public virtual WarehouseExportDetail? WarehouseExportDetail { get; set; }
    public virtual Product? Product { get; set; }
}

internal class WarehouseRefundDetailConfig : IEntityTypeConfiguration<WarehouseRefundDetail> {

    public void Configure(EntityTypeBuilder<WarehouseRefundDetail> builder) {
        builder.ToTable(nameof(WarehouseRefundDetail));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32).IsRequired();

        builder.Property(o => o.WarehouseRefundId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.ProductId).HasMaxLength(32).IsRequired();

        // index
        builder.HasIndex(o => o.WarehouseRefundId);

        // fk
        builder.HasOne(o => o.WarehouseRefund).WithMany(o => o.WarehouseRefundDetails).HasForeignKey(o => o.WarehouseRefundId);
        builder.HasOne(o => o.WarehouseExportDetail).WithMany(o => o.WarehouseRefundDetails).HasForeignKey(o => o.WarehouseExportDetailId);
        builder.HasOne(o => o.Product).WithMany(o => o.WarehouseRefundDetails).HasForeignKey(o => o.ProductId);
    }
}
