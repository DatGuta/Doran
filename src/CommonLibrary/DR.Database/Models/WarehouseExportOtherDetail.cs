namespace DR.Database.Models;

public partial class WarehouseExportOtherDetail {
    public string Id { get; set; } = null!;
    public string WarehouseExportOtherId { get; set; } = null!;
    public string ProductId { get; set; } = null!;
    public decimal Quantity { get; set; }

    public virtual WarehouseExportOther? WarehouseExportOther { get; set; }
    public virtual Product? Product { get; set; }
}

internal class WarehouseExportOtherDetailConfig : IEntityTypeConfiguration<WarehouseExportOtherDetail> {

    public void Configure(EntityTypeBuilder<WarehouseExportOtherDetail> builder) {
        builder.ToTable(nameof(WarehouseExportOtherDetail));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32).IsRequired();

        builder.Property(o => o.WarehouseExportOtherId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.ProductId).HasMaxLength(32).IsRequired();

        // index
        builder.HasIndex(o => o.WarehouseExportOtherId);

        // fk
        builder.HasOne(o => o.WarehouseExportOther).WithMany(o => o.WarehouseExportOtherDetails).HasForeignKey(o => o.WarehouseExportOtherId);
        builder.HasOne(o => o.Product).WithMany(o => o.WarehouseExportOtherDetails).HasForeignKey(o => o.ProductId);
    }
}
