namespace DR.Database.Models;

public partial class CustomerPhone {
    public string Id { get; set; } = null!;
    public string CustomerId { get; set; } = null!;
    public string? SearchPhone { get; set; }
    public string? SearchLastPhone { get; set; }

    public virtual Customer? Customer { get; set; }
}

internal class CustomerPhoneConfig : IEntityTypeConfiguration<CustomerPhone> {

    public void Configure(EntityTypeBuilder<CustomerPhone> builder) {
        builder.ToTable(nameof(CustomerPhone));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);
        builder.Property(o => o.CustomerId).HasMaxLength(32).IsRequired();

        // index
        builder.HasIndex(o => o.CustomerId);

        // fk
        builder.HasOne(o => o.Customer).WithMany(o => o.CustomerPhones).HasForeignKey(o => o.CustomerId);
    }
}
