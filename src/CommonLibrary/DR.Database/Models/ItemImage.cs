using DR.Constant.Enums;

namespace DR.Database.Models;

public partial class ItemImage : IEntity {
    public string Id { get; set; } = null!;
    public string MerchantId { get; set; } = null!;
    public string ItemId { get; set; } = null!;
    public EItemImage ItemType { get; set; }
    public string Name { get; set; } = null!;
    public string Image { get; set; } = null!;
}

internal class ItemImageConfig : IEntityTypeConfiguration<ItemImage> {

    public void Configure(EntityTypeBuilder<ItemImage> builder) {
        builder.ToTable(nameof(ItemImage));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);

        builder.Property(o => o.MerchantId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.ItemId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.Name).HasMaxLength(255).IsRequired();
        builder.Property(o => o.ItemType).HasMaxLength(20);
        builder.Property(o => o.Image).HasMaxLength(8000);

        // index
        builder.HasIndex(o => o.MerchantId);
        builder.HasIndex(o => new { o.MerchantId, o.ItemId });
        builder.HasIndex(o => new { o.MerchantId, o.ItemId, o.ItemType });
    }
}
