namespace DR.Database.Models;

public class CustomerGroup {
    public string Id { get; set; } = null!;
    public string MerchantId { get; set; } = null!;

    [Description("Mã nhóm khách hàng")]
    public string Code { get; set; } = null!;

    [Description("Tên nhóm khách hàng")]
    public string Name { get; set; } = null!;

    public string SearchName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int NumberOfCustomers { get; set; }

    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ModifiedDate { get; set; } = DateTimeOffset.UtcNow;
    public bool IsDelete { get; set; }

    public virtual ICollection<Customer>? Customers { get; set; }
}

internal class CustomerGroupConfig : IEntityTypeConfiguration<CustomerGroup> {

    public void Configure(EntityTypeBuilder<CustomerGroup> builder) {
        builder.ToTable(nameof(CustomerGroup));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32).IsRequired();
        builder.Property(o => o.MerchantId).HasMaxLength(32).IsRequired();

        builder.Property(o => o.Code).HasMaxLength(50).IsRequired();
        builder.Property(o => o.Name).HasMaxLength(255).IsRequired();
        builder.Property(o => o.SearchName).HasMaxLength(255).IsRequired();

        builder.Property(o => o.Description).HasMaxLength(2000).IsRequired();

        builder.Property(o => o.CreatedDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o));
        builder.Property(o => o.ModifiedDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o));

        //fk

        builder.HasMany(o => o.Customers).WithOne(o => o.CustomerGroup).HasForeignKey(o => o.CustomerGroupId);
    }
}
