namespace DR.Database.Models;

public partial class ProductSupplier : ISyncEntity {
    public string Id { get; set; } = null!;
    public string MerchantId { get; set; } = null!;
    public string SupplierId { get; set; } = null!;
    public string ProductId { get; set; } = null!;

    public bool IsDelete { get; set; }
    public DateTimeOffset ModifiedDate { get; set; } = DateTimeOffset.UtcNow;
}

internal class ProductSupplierConfig : IEntityTypeConfiguration<ProductSupplier> {

    public void Configure(EntityTypeBuilder<ProductSupplier> builder) {
        builder.ToTable(nameof(ProductSupplier));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);
        builder.Property(o => o.MerchantId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.ProductId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.SupplierId).HasMaxLength(32).IsRequired();

        builder.Property(o => o.ModifiedDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();

        // index
        builder.HasIndex(o => o.MerchantId);
        builder.HasIndex(o => new { o.MerchantId, o.SupplierId });
        builder.HasIndex(o => new { o.MerchantId, o.ProductId });
    }
}
