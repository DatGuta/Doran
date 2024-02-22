namespace DR.Database.Models;

public partial class ProductOnStore : ISyncEntity {
    public string Id { get; set; } = null!;
    public string MerchantId { get; set; } = null!;

    [Description("Mã cửa hàng")]
    public string StoreId { get; set; } = null!;

    [Description("Mã sản phẩm")]
    public string ProductId { get; set; } = null!;

    public bool IsDelete { get; set; }
    public DateTimeOffset ModifiedDate { get; set; } = DateTimeOffset.UtcNow;

    public virtual Store? Store { get; set; }
    public virtual Product? Product { get; set; }
}

internal class ProductOnStoreConfig : IEntityTypeConfiguration<ProductOnStore> {

    public void Configure(EntityTypeBuilder<ProductOnStore> builder) {
        builder.ToTable(nameof(ProductOnStore));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32).IsRequired();
        builder.Property(o => o.MerchantId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.StoreId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.ProductId).HasMaxLength(32).IsRequired();

        builder.Property(o => o.ModifiedDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();

        // index
        builder.HasIndex(o => o.MerchantId);
        builder.HasIndex(o => new { o.MerchantId, o.StoreId });
        builder.HasIndex(o => new { o.MerchantId, o.StoreId, o.ProductId }).IsUnique();

        // fk
        builder.HasOne(o => o.Store).WithMany(o => o.ProductOnStores).HasForeignKey(o => o.StoreId);
        builder.HasOne(o => o.Product).WithMany(o => o.ProductOnStores).HasForeignKey(o => o.ProductId);
    }
}
