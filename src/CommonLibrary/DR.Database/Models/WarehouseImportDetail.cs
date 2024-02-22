namespace DR.Database.Models;

public partial class WarehouseImportDetail {
    public string Id { get; set; } = null!;
    public string WarehouseImportId { get; set; } = null!;

    [Description("Sản phẩm")]
    public string ProductId { get; set; } = null!;

    [Description("Số lượng theo chứng từ")]
    public decimal SupplierOrderQuantity { get; set; }

    [Description("Số lượng thực nhập")]
    public decimal Quantity { get; set; }

    [Description("Đơn giá")]
    public decimal Cost { get; set; }

    public decimal SubTotal { get; set; }

    public virtual WarehouseImport? WarehouseImport { get; set; }
    public virtual Product? Product { get; set; }
}

internal class WarehouseImportDetailConfig : IEntityTypeConfiguration<WarehouseImportDetail> {

    public void Configure(EntityTypeBuilder<WarehouseImportDetail> builder) {
        builder.ToTable(nameof(WarehouseImportDetail));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32).IsRequired();

        builder.Property(o => o.WarehouseImportId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.ProductId).HasMaxLength(32).IsRequired();

        // index
        builder.HasIndex(o => o.WarehouseImportId);

        // fk
        builder.HasOne(o => o.WarehouseImport).WithMany(o => o.WarehouseImportDetails).HasForeignKey(o => o.WarehouseImportId);
        builder.HasOne(o => o.Product).WithMany(o => o.WarehouseImportDetails).HasForeignKey(o => o.ProductId);
    }
}
