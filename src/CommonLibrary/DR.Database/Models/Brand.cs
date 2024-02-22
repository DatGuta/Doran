

namespace DR.Database.Models;

public partial class Brand : ISyncEntity {
    public string Id { get; set; } = null!;
    public string MerchantId { get; set; } = null!;

    [Description("Mã thương hiệu")]
    public string Code { get; set; } = null!;

    [Description("Tên thương hiệu")]
    public string Name { get; set; } = null!;

    public string SearchName { get; set; } = null!;

    [Description("Số điện thoại")]
    public string? Phone { get; set; }

    [Description("Email")]
    public string? Email { get; set; }

    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ModifiedDate { get; set; } = DateTimeOffset.UtcNow;
    public bool IsDelete { get; set; }

    public virtual ICollection<ProductBrand>? ProductBrands { get; set; }
}

public partial class Brand {

    public void UpdateFrom(Brand entity) {
        this.Name = entity.Name;
        this.SearchName = entity.SearchName;
        this.Phone = entity.Phone;
        this.Email = entity.Email;
        this.ModifiedDate = DateTimeOffset.UtcNow;
    }
}

internal class BrandConfig : IEntityTypeConfiguration<Brand> {

    public void Configure(EntityTypeBuilder<Brand> builder) {
        builder.ToTable(nameof(Brand));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);
        builder.Property(o => o.MerchantId).HasMaxLength(32).IsRequired();

        builder.Property(o => o.Code).HasMaxLength(50).IsRequired();
        builder.Property(o => o.Name).HasMaxLength(255).IsRequired();
        builder.Property(o => o.SearchName).HasMaxLength(255).IsRequired();

        builder.Property(o => o.CreatedDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o));
        builder.Property(o => o.ModifiedDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o));

        // index
        builder.HasIndex(o => o.MerchantId);
        builder.HasIndex(o => new { o.MerchantId, o.Code }).IsUnique();

        // fk
        builder.HasMany(o => o.ProductBrands).WithOne(o => o.Brand).HasForeignKey(o => o.BrandId);
    }
}
