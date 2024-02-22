namespace DR.Database.Models;

public partial class ProductOnWarehouse : ISyncEntity {
    public string Id { get; set; } = null!;
    public string MerchantId { get; set; } = null!;
    public string WarehouseId { get; set; } = null!;
    public string ProductId { get; set; } = null!;

    public decimal OnHand { get; set; }
    public bool IsActive { get; set; }

    public DateTimeOffset ModifiedDate { get; set; } = DateTimeOffset.UtcNow;

    public virtual Warehouse? Warehouse { get; set; }
    public virtual Product? Product { get; set; }
}

internal class ProductOnWarehouseConfig : IEntityTypeConfiguration<ProductOnWarehouse> {

    public void Configure(EntityTypeBuilder<ProductOnWarehouse> builder) {
        builder.ToTable(nameof(ProductOnWarehouse));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);
        builder.Property(o => o.MerchantId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.WarehouseId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.ProductId).HasMaxLength(32).IsRequired();

        builder.Property(o => o.ModifiedDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();

        // index
        builder.HasIndex(o => o.MerchantId);
        builder.HasIndex(o => new { o.MerchantId, o.WarehouseId });
        builder.HasIndex(o => new { o.MerchantId, o.WarehouseId, o.ProductId }).IsUnique();

        // fk
        builder.HasOne(o => o.Warehouse).WithMany(o => o.ProductOnWarehouses).HasForeignKey(o => o.WarehouseId);
        builder.HasOne(o => o.Product).WithMany(o => o.ProductOnWarehouses).HasForeignKey(o => o.ProductId);
    }
}
