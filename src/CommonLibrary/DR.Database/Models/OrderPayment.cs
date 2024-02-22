namespace DR.Database.Models;

public partial class OrderPayment {
    public string Id { get; set; } = null!;
    public string OrderId { get; set; } = null!;
    public string PaymentMethodId { get; set; } = null!;

    public decimal Value { get; set; }
    public string? Description { get; set; }

    public string CreatedBy { get; set; } = null!;
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

    public virtual Order? Order { get; set; }
    public virtual PaymentMethod? PaymentMethod { get; set; }
    public virtual User? CreatedUser { get; set; }
}

internal class OrderPaymentConfig : IEntityTypeConfiguration<OrderPayment> {

    public void Configure(EntityTypeBuilder<OrderPayment> builder) {
        builder.ToTable(nameof(OrderPayment));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);

        builder.Property(o => o.OrderId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.PaymentMethodId).HasMaxLength(32).IsRequired();

        builder.Property(o => o.Description).HasMaxLength(2000);

        builder.Property(o => o.CreatedDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o));

        // index
        builder.HasIndex(o => o.OrderId);
        builder.HasIndex(o => o.PaymentMethodId);
        builder.HasIndex(o => new { o.OrderId, o.PaymentMethodId });

        // fk
        builder.HasOne(o => o.Order).WithMany(o => o.OrderPayments).HasForeignKey(o => o.OrderId);
        builder.HasOne(o => o.PaymentMethod).WithMany(o => o.OrderPayments).HasForeignKey(o => o.PaymentMethodId);
        builder.HasOne(o => o.CreatedUser).WithMany(o => o.OrderPayments).HasForeignKey(o => o.CreatedBy);
    }
}
