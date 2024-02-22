namespace DR.Database.Models;

public partial class Receipt : IEntity {
    public string Id { get; set; } = null!;
    public string MerchantId { get; set; } = null!;

    [Description("Mã phiếu thu")]
    public string Code { get; set; } = null!;

    [Description("Khách hàng")]
    public string CustomerId { get; set; } = null!;

    [Description("Phương thức thanh toán")]
    public string PaymentMethodId { get; set; } = null!;

    [Description("Lý do")]
    public string Description { get; set; } = null!;

    [Description("Thu tiền")]
    public decimal Value { get; set; }

    [Description("Ngày thu")]
    public DateTimeOffset ReceiptDate { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ModifiedDate { get; set; } = DateTimeOffset.UtcNow;
    public string CreatedBy { get; set; } = null!;
    public bool IsDelete { get; set; }

    public virtual Customer? Customer { get; set; }
    public virtual PaymentMethod? PaymentMethod { get; set; }

    public virtual ICollection<ReceiptDetail>? ReceiptDetails { get; set; }
}

internal class ReceiptConfig : IEntityTypeConfiguration<Receipt> {

    public void Configure(EntityTypeBuilder<Receipt> builder) {
        builder.ToTable(nameof(Receipt));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);
        builder.Property(o => o.MerchantId).HasMaxLength(32).IsRequired();

        builder.Property(o => o.CustomerId).HasMaxLength(32);
        builder.Property(o => o.PaymentMethodId).HasMaxLength(32);

        builder.Property(o => o.Code).HasMaxLength(50).IsRequired();

        builder.Property(o => o.Description).HasMaxLength(2000);

        builder.Property(o => o.ReceiptDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();
        builder.Property(o => o.CreatedDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();
        builder.Property(o => o.ModifiedDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();
        builder.Property(o => o.CreatedBy).HasMaxLength(32);

        // index
        builder.HasIndex(o => o.MerchantId);
        builder.HasIndex(o => new { o.MerchantId, o.Code }).IsUnique();
        builder.HasIndex(o => new { o.MerchantId, o.CustomerId });

        // fk
        builder.HasOne(o => o.Customer).WithMany(o => o.Receipts).HasForeignKey(o => o.CustomerId);
        builder.HasOne(o => o.PaymentMethod).WithMany(o => o.Receipts).HasForeignKey(o => o.PaymentMethodId);

        builder.HasMany(o => o.ReceiptDetails).WithOne(o => o.Receipt).HasForeignKey(o => o.ReceiptId);
    }
}
