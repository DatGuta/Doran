namespace DR.Database.Models;

public partial class Merchant {
    public string Id { get; set; } = null!;

    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTimeOffset ExpiredDate { get; set; } = DateTimeOffset.UtcNow;
    public string? ApiSecret { get; set; }
    public long? At { get; set; }
}

internal class MerchantConfig : IEntityTypeConfiguration<Merchant> {

    public void Configure(EntityTypeBuilder<Merchant> builder) {
        builder.ToTable(nameof(Merchant));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);

        builder.Property(o => o.Code).HasMaxLength(50).IsRequired();
        builder.Property(o => o.Name).HasMaxLength(255).IsRequired();
        builder.Property(o => o.ApiSecret).HasMaxLength(255);
        builder.Property(o => o.ExpiredDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();

        builder.HasIndex(o => o.Code).IsUnique();
    }
}
