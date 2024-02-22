using DR.Constant.Enums;

namespace DR.Database.Models;

public partial class CustomerTracking : IEntity {
    public string Id { get; set; } = null!;
    public string MerchantId { get; set; } = null!;
    public string CustomerId { get; set; } = null!;

    public ECustomerDocType DocumentType { get; set; }
    public string DocumentId { get; set; } = null!;
    public string DocumentCode { get; set; } = null!;

    public decimal DebtBefore { get; set; }
    public decimal Debt { get; set; }
    public decimal DebtAfter { get; set; }

    public decimal BalanceBefore { get; set; }
    public decimal Balance { get; set; }
    public decimal BalanceAfter { get; set; }

    public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;
    public bool IsUpdate { get; set; }
    public bool IsDelete { get; set; }

    public virtual Customer? Customer { get; set; }
}

internal class CustomerDebtTrackingConfig : IEntityTypeConfiguration<CustomerTracking> {

    public void Configure(EntityTypeBuilder<CustomerTracking> builder) {
        builder.ToTable(nameof(CustomerTracking));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);
        builder.Property(o => o.MerchantId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.CustomerId).HasMaxLength(32).IsRequired();

        builder.Property(o => o.DocumentType).IsRequired();
        builder.Property(o => o.DocumentId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.DocumentCode).HasMaxLength(255).IsRequired();

        builder.Property(o => o.Date).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();

        // fk
        builder.HasOne(o => o.Customer).WithMany(o => o.CustomerTrackings).HasForeignKey(o => o.CustomerId);

        // index
        builder.HasIndex(o => o.MerchantId);
        builder.HasIndex(o => new { o.MerchantId, o.CustomerId });
        builder.HasIndex(o => new { o.MerchantId, o.CustomerId, o.IsUpdate });
        builder.HasIndex(o => new { o.MerchantId, o.CustomerId, o.IsUpdate, o.IsDelete });
    }
}
