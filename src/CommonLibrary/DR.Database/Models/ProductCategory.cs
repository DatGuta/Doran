namespace DR.Database.Models;

public partial class ProductCategory : ISyncEntity {
    public string Id { get; set; } = null!;
    public string MerchantId { get; set; } = null!;
    public string CategoryId { get; set; } = null!;
    public string ProductId { get; set; } = null!;

    public bool IsDelete { get; set; }
    public DateTimeOffset ModifiedDate { get; set; } = DateTimeOffset.UtcNow;

    public virtual Product? Product { get; set; }
    public virtual Category? Category { get; set; }
}

internal class ProductCategoryConfig : IEntityTypeConfiguration<ProductCategory> {

    public void Configure(EntityTypeBuilder<ProductCategory> builder) {
        builder.ToTable(nameof(ProductCategory));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);
        builder.Property(o => o.MerchantId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.ProductId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.CategoryId).HasMaxLength(32).IsRequired();

        builder.Property(o => o.ModifiedDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();

        // index
        builder.HasIndex(o => o.MerchantId);
        builder.HasIndex(o => new { o.MerchantId, o.CategoryId });
        builder.HasIndex(o => new { o.MerchantId, o.ProductId });

        // fk
        builder.HasOne(o => o.Category).WithMany(o => o.ProductCategories).HasForeignKey(o => o.CategoryId);
        builder.HasOne(o => o.Product).WithMany(o => o.ProductCategories).HasForeignKey(o => o.ProductId);
    }
}
