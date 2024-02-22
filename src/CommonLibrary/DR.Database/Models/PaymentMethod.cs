using DR.Constant.Enums;

namespace DR.Database.Models;

public partial class PaymentMethod : IEntity {
    public string Id { get; set; } = null!;
    public string MerchantId { get; set; } = null!;
    public string SearchName { get; set; } = null!;

    [Description("Mã phương thức")]
    public string Code { get; set; } = null!;

    [Description("Tên phương thức")]
    public string Name { get; set; } = null!;

    [Description("Loại phương thức")]
    public EPaymentMethodType Type { get; set; }

    [Description("Đặt làm mặc định")]
    public bool IsDefault { get; set; }

    public bool IsDelete { get; set; }
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
    public virtual ICollection<OrderPayment>? OrderPayments { get; set; }
    public virtual ICollection<Receipt>? Receipts { get; set; }
    public virtual ICollection<Payment>? Payments { get; set; }
}

internal class PaymentMethodConfig : IEntityTypeConfiguration<PaymentMethod> {

    public void Configure(EntityTypeBuilder<PaymentMethod> builder) {
        builder.ToTable(nameof(PaymentMethod));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);
        builder.Property(O => O.MerchantId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.Code).HasMaxLength(50).IsRequired();
        builder.Property(o => o.Name).HasMaxLength(255).IsRequired();
        builder.Property(o => o.SearchName).HasMaxLength(255).IsRequired();

        builder.Property(o => o.CreatedDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o));

        //index
        builder.HasIndex(o => o.MerchantId);
        builder.HasIndex(o => new { o.MerchantId, o.Code }).IsUnique();

        //fk
        builder.HasMany(o => o.OrderPayments).WithOne(o => o.PaymentMethod).HasForeignKey(o => o.PaymentMethodId);
        builder.HasMany(o => o.Receipts).WithOne(o => o.PaymentMethod).HasForeignKey(o => o.PaymentMethodId);
        builder.HasMany(o => o.Payments).WithOne(o => o.PaymentMethod).HasForeignKey(o => o.PaymentMethodId);
    }
}
