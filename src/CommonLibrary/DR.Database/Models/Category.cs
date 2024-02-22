namespace DR.Database.Models;

public partial class Category : ISyncEntity {
    public string Id { get; set; } = null!;
    public string MerchantId { get; set; } = null!;

    [Description("Mã danh mục")]
    public string Code { get; set; } = null!;

    [Description("Tên danh mục")]
    public string Name { get; set; } = null!;

    public string SearchName { get; set; } = null!;
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ModifiedDate { get; set; } = DateTimeOffset.UtcNow;
    public bool IsDelete { get; set; } = false;

    public virtual ICollection<ProductCategory>? ProductCategories { get; set; }
}

internal class CategoryConfig : IEntityTypeConfiguration<Category> {

    public void Configure(EntityTypeBuilder<Category> builder) {
        builder.ToTable(nameof(Category));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);
        builder.Property(O => O.MerchantId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.Code).HasMaxLength(50).IsRequired();
        builder.Property(o => o.Name).HasMaxLength(255).IsRequired();
        builder.Property(o => o.SearchName).HasMaxLength(255).IsRequired();
        builder.Property(o => o.CreatedDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o));
        builder.Property(o => o.ModifiedDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o));

        //index
        builder.HasIndex(o => o.MerchantId);
        builder.HasIndex(o => new { o.MerchantId, o.Code }).IsUnique();

        //fk
        builder.HasMany(o => o.ProductCategories).WithOne(o => o.Category).HasForeignKey(o => o.CategoryId);
    }
}
