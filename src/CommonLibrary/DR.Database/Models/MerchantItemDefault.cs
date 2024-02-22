using DR.Constant.Enums;

namespace DR.Database.Models;

public class MerchantItemDefault {
    public string Id { get; set; } = null!;
    public string MerchantId { get; set; } = null!;
    public string? ItemId { get; set; }
    public EMerchantItemDefault Type { get; set; }
}

internal class MerchantItemDefaultConfig : IEntityTypeConfiguration<MerchantItemDefault> {

    public void Configure(EntityTypeBuilder<MerchantItemDefault> builder) {
        builder.ToTable(nameof(MerchantItemDefault));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32).IsFixedLength();
        builder.Property(o => o.MerchantId).HasMaxLength(32).IsFixedLength();
        builder.Property(o => o.ItemId).HasMaxLength(32).IsFixedLength();

        builder.HasIndex(o => new { o.Type, o.MerchantId }).IsUnique();
    }
}
