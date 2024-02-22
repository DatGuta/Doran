namespace DR.Database.Models;

public partial class WarehouseTransferDetail {
    public string Id { get; set; } = null!;
    public string WarehouseTransferId { get; set; } = null!;
    public string ProductId { get; set; } = null!;

    [Description("Số lượng xuất")]
    public decimal ExportedQuantity { get; set; }

    [Description("Số lượng nhập")]
    public decimal ImportQuantity { get; set; }

    public virtual WarehouseTransfer? WarehouseTransfer { get; set; }
    public virtual Product? Product { get; set; }
}

internal class WarehouseTransferDetailConfig : IEntityTypeConfiguration<WarehouseTransferDetail> {

    public void Configure(EntityTypeBuilder<WarehouseTransferDetail> builder) {
        builder.ToTable(nameof(WarehouseTransferDetail));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32).IsRequired();

        builder.Property(o => o.WarehouseTransferId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.ProductId).HasMaxLength(32).IsRequired();

        // index
        builder.HasIndex(o => o.WarehouseTransferId);

        // fk
        builder.HasOne(o => o.WarehouseTransfer).WithMany(o => o.WarehouseTransferDetails).HasForeignKey(o => o.WarehouseTransferId);
        builder.HasOne(o => o.Product).WithMany(o => o.WarehouseTransferDetails).HasForeignKey(o => o.ProductId);
    }
}
