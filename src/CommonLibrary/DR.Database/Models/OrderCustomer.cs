namespace DR.Database.Models;

public partial class OrderCustomer {
    public string Id { get; set; } = null!;
    public string OrderId { get; set; } = null!;

    [Description("Tên người nhận")]
    public string Name { get; set; } = null!;

    [Description("Số điện thoại")]
    public string Phone { get; set; } = null!;

    [Description("Tỉnh/Thành phố")]
    public string? Province { get; set; }

    [Description("Quận/Huyện")]
    public string? District { get; set; }

    [Description("Phường/Xã")]
    public string? Commune { get; set; }

    [Description("Địa chỉ")]
    public string? Address { get; set; }

    public string? Note { get; set; }

    public virtual Order? Order { get; set; }
}

internal class OrderCustomerConfig : IEntityTypeConfiguration<OrderCustomer> {

    public void Configure(EntityTypeBuilder<OrderCustomer> builder) {
        builder.ToTable(nameof(OrderCustomer));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);
        builder.Property(o => o.OrderId).HasMaxLength(32).IsRequired();

        builder.Property(o => o.Name).HasMaxLength(255);
        builder.Property(o => o.Province).HasMaxLength(20);
        builder.Property(o => o.District).HasMaxLength(20);
        builder.Property(o => o.Commune).HasMaxLength(20);
        builder.Property(o => o.Address).HasMaxLength(255);

        // index
        builder.HasIndex(o => o.OrderId);

        // fk
        builder.HasOne(o => o.Order).WithMany(o => o.OrderCustomers).HasForeignKey(o => o.OrderId);
    }
}
