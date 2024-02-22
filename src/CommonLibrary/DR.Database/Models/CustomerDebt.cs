namespace DR.Database.Models;
public partial class CustomerDebt : IEntity {
    public string Id { get; set; } = null!;
    public string MerchantId { get; set; } = null!;

    public decimal AccountAmount { get; set; }
}

internal class CustomerDebtConfig : IEntityTypeConfiguration<CustomerDebt> {

    public void Configure(EntityTypeBuilder<CustomerDebt> builder) {
        builder.ToTable(nameof(CustomerDebt));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);
        builder.Property(o => o.MerchantId).HasMaxLength(32).IsRequired();

        // index
        builder.HasIndex(o => o.MerchantId);
    }
}
