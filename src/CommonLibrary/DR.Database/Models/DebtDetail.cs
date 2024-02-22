namespace DR.Database.Models;

public partial class DebtDetail {
    public string Id { get; set; } = null!;
    public string DebtId { get; set; } = null!;

    public string ItemId { get; set; } = null!;

    public decimal Value { get; set; }
    public DateTimeOffset Time { get; set; }

    public virtual Debt? Debt { get; set; }
}

internal class DebtDetailConfig : IEntityTypeConfiguration<DebtDetail> {

    public void Configure(EntityTypeBuilder<DebtDetail> builder) {
        builder.ToTable(nameof(DebtDetail));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);
        builder.Property(o => o.DebtId).HasMaxLength(32);

        builder.Property(o => o.ItemId).HasMaxLength(50).IsRequired();
        builder.Property(o => o.Time).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o));

        // index
        builder.HasIndex(o => o.DebtId);

        // fk
        builder.HasOne(o => o.Debt).WithMany(o => o.DebtDetails).HasForeignKey(o => o.DebtId);
    }
}
