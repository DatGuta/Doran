namespace DR.Database.Models;

public partial class Customer : ISyncEntity {
    public string Id { get; set; } = null!;
    public string MerchantId { get; set; } = null!;

    [Description("Mã khách hàng")]
    public string Code { get; set; } = null!;

    [Description("Tên khách hàng")]
    public string Name { get; set; } = null!;

    public string SearchName { get; set; } = null!;

    [Description("Số điện thoại")]
    public string? Phone { get; set; }

    [Description("Tỉnh/Thành phố")]
    public string? Province { get; set; }

    [Description("Quận/Huyện")]
    public string? District { get; set; }

    [Description("Phường/Xã")]
    public string? Commune { get; set; }

    [Description("Địa chỉ")]
    public string? Address { get; set; }

    public long LastPurchase { get; set; } = -1;

    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ModifiedDate { get; set; } = DateTimeOffset.UtcNow;
    public bool IsDelete { get; set; }

    [Description("Nhóm khách hàng")]
    public string? CustomerGroupId { get; set; }

    public virtual CustomerGroup? CustomerGroup { get; set; }

    public virtual ICollection<CustomerPhone>? CustomerPhones { get; set; }
    public virtual ICollection<Order>? Orders { get; set; }
    public virtual ICollection<WarehouseExport>? WarehouseExports { get; set; }
    public virtual ICollection<Debt>? Debts { get; set; }
    public virtual ICollection<Receipt>? Receipts { get; set; }
    public virtual ICollection<Payment>? Payments { get; set; }
    public virtual ICollection<CustomerTracking>? CustomerTrackings { get; set; }
}

public partial class Customer {

    public void UpdateFrom(Customer entity) {
        this.Name = entity.Name;
        this.SearchName = entity.SearchName;
        this.CustomerGroupId = entity.CustomerGroupId;
        this.Phone = entity.Phone;
        this.Province = entity.Province;
        this.District = entity.District;
        this.Commune = entity.Commune;
        this.Address = entity.Address;
        this.ModifiedDate = DateTimeOffset.UtcNow;
    }
}

internal class CustomerConfig : IEntityTypeConfiguration<Customer> {

    public void Configure(EntityTypeBuilder<Customer> builder) {
        builder.ToTable(nameof(Customer));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);
        builder.Property(o => o.MerchantId).HasMaxLength(32).IsRequired();

        builder.Property(o => o.Code).HasMaxLength(50).IsRequired();
        builder.Property(o => o.Name).HasMaxLength(255).IsRequired();
        builder.Property(o => o.SearchName).HasMaxLength(255).IsRequired();

        builder.Property(o => o.Province).HasMaxLength(20);
        builder.Property(o => o.District).HasMaxLength(20);
        builder.Property(o => o.Commune).HasMaxLength(20);
        builder.Property(o => o.Address).HasMaxLength(255);
        builder.Property(o => o.CreatedDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o));
        builder.Property(o => o.ModifiedDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o));

        // index
        builder.HasIndex(o => o.MerchantId);
        builder.HasIndex(o => new { o.MerchantId, o.Code }).IsUnique();

        // fk
        builder.HasOne(o => o.CustomerGroup).WithMany(o => o.Customers).HasForeignKey(o => o.CustomerGroupId);

        builder.HasMany(o => o.CustomerPhones).WithOne(o => o.Customer).HasForeignKey(o => o.CustomerId);
        builder.HasMany(o => o.Orders).WithOne(o => o.Customer).HasForeignKey(o => o.CustomerId);
        builder.HasMany(o => o.WarehouseExports).WithOne(o => o.Customer).HasForeignKey(o => o.CustomerId);
        builder.HasMany(o => o.Debts).WithOne(o => o.Customer).HasForeignKey(o => o.CustomerId);
        builder.HasMany(o => o.Receipts).WithOne(o => o.Customer).HasForeignKey(o => o.CustomerId);
        builder.HasMany(o => o.Payments).WithOne(o => o.Customer).HasForeignKey(o => o.CustomerId);
        builder.HasMany(o => o.CustomerTrackings).WithOne(o => o.Customer).HasForeignKey(o => o.CustomerId);
    }
}
