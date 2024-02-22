using DR.Constant.Enums;

namespace DR.Database.Models;

public partial class Debt : IEntity {
    public string Id { get; set; } = null!;
    public string MerchantId { get; set; } = null!;
    public string CustomerId { get; set; } = null!;
    public string UserId { get; set; } = null!;

    public EDebtItem ItemType { get; set; }

    [Description("Số tiền")]
    public decimal Value { get; set; }

    [Description("Ghi chú")]
    public string? Description { get; set; }

    public bool IsOriginal { get; set; }
    public bool IsDelete { get; set; }

    public DateTimeOffset Time { get; set; } = DateTimeOffset.UtcNow;

    public virtual Customer? Customer { get; set; }
    public virtual User? User { get; set; }
    public virtual ICollection<DebtDetail>? DebtDetails { get; set; }
}

internal class DebtConfig : IEntityTypeConfiguration<Debt> {

    public void Configure(EntityTypeBuilder<Debt> builder) {
        builder.ToTable(nameof(Debt));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);
        builder.Property(o => o.MerchantId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.CustomerId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.UserId).HasMaxLength(32).IsRequired();

        builder.Property(o => o.Description).HasMaxLength(2000);

        builder.Property(o => o.Time).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o));

        // index
        builder.HasIndex(o => o.MerchantId);
        builder.HasIndex(o => new { o.MerchantId, o.CustomerId });

        // fk
        builder.HasOne(o => o.Customer).WithMany(o => o.Debts).HasForeignKey(o => o.CustomerId);
        builder.HasOne(o => o.User).WithMany(o => o.Debts).HasForeignKey(o => o.UserId);
        builder.HasMany(o => o.DebtDetails).WithOne(o => o.Debt).HasForeignKey(o => o.DebtId);
    }
}
