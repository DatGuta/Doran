using DR.Constant.Enums;

namespace DR.Database.Models;

public partial class Payment : IEntity {
    public string Id { get; set; } = null!;
    public string MerchantId { get; set; } = null!;

    [Description("Mã phiếu chi")]
    public string Code { get; set; } = null!;

    [Description("Khách hàng")]
    public string CustomerId { get; set; } = null!;

    [Description("Tên người nhận")]
    public string? RecipientName { get; set; }

    [Description("Số điện thoại người nhận")]
    public string? RecipientPhone { get; set; }

    [Description("Địa chỉ người nhận")]
    public string? RecipientAddress { get; set; }

    [Description("Tổng tiền")]
    public decimal Value { get; set; }

    [Description("Loại phiếu chi")]
    public EPayment Type { get; set; }

    [Description("Lý do")]
    public string Description { get; set; } = null!;

    [Description("Phương thức thanh toán")]
    public string PaymentMethodId { get; set; } = null!;

    public bool IsDelete { get; set; }

    [Description("Ngày tạo")]
    public DateTimeOffset PaymentDate { get; set; }

    public string CreatedBy { get; set; } = null!;
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ModifiedDate { get; set; } = DateTimeOffset.UtcNow;

    public virtual Customer? Customer { get; set; }
    public virtual PaymentMethod? PaymentMethod { get; set; }

    public virtual ICollection<PaymentDetail>? PaymentDetails { get; set; }
}

public class PaymentConfig : IEntityTypeConfiguration<Payment> {

    public void Configure(EntityTypeBuilder<Payment> builder) {
        builder.ToTable(nameof(Payment));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);
        builder.Property(o => o.MerchantId).HasMaxLength(32).IsRequired();

        builder.Property(o => o.CustomerId).HasMaxLength(32);
        builder.Property(o => o.PaymentMethodId).HasMaxLength(32);

        builder.Property(o => o.Code).HasMaxLength(50).IsRequired();

        builder.Property(o => o.Description).HasMaxLength(2000);

        builder.Property(o => o.PaymentDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();
        builder.Property(o => o.CreatedDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();
        builder.Property(o => o.ModifiedDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();
        builder.Property(o => o.CreatedBy).HasMaxLength(32);

        // index
        builder.HasIndex(o => o.MerchantId);
        builder.HasIndex(o => new { o.MerchantId, o.Code }).IsUnique();
        builder.HasIndex(o => new { o.MerchantId, o.CustomerId });

        // fk
        builder.HasOne(o => o.Customer).WithMany(o => o.Payments).HasForeignKey(o => o.CustomerId);
        builder.HasOne(o => o.PaymentMethod).WithMany(o => o.Payments).HasForeignKey(o => o.PaymentMethodId);

        builder.HasMany(o => o.PaymentDetails).WithOne(o => o.Payment).HasForeignKey(o => o.PaymentId);
    }
}
