namespace DR.Database.Models;

public partial class ProductBrand : ISyncEntity {
    public string Id { get; set; } = null!;
    public string MerchantId { get; set; } = null!;
    public string BrandId { get; set; } = null!;
    public string ProductId { get; set; } = null!;

    public bool IsDelete { get; set; }
    public DateTimeOffset ModifiedDate { get; set; } = DateTimeOffset.UtcNow;

    public virtual Product? Product { get; set; }
    public virtual Brand? Brand { get; set; }
}

internal class ProductBrandConfig : IEntityTypeConfiguration<ProductBrand> {

    public void Configure(EntityTypeBuilder<ProductBrand> builder) {
        builder.ToTable(nameof(ProductBrand));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);
        builder.Property(o => o.MerchantId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.ProductId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.BrandId).HasMaxLength(32).IsRequired();

        builder.Property(o => o.ModifiedDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();

        // index
        builder.HasIndex(o => o.MerchantId);
        builder.HasIndex(o => new { o.MerchantId, o.BrandId });
        builder.HasIndex(o => new { o.MerchantId, o.ProductId });

        // fk
        builder.HasOne(o => o.Brand).WithMany(o => o.ProductBrands).HasForeignKey(o => o.BrandId);
        builder.HasOne(o => o.Product).WithMany(o => o.ProductBrands).HasForeignKey(o => o.ProductId);
    }
}
