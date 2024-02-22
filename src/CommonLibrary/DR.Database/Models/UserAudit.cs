using DR.Constant.Enums;

namespace DR.Database.Models;

public partial class UserAudit : IEntity {
    public string Id { get; set; } = null!;
    public string MerchantId { get; set; } = null!;
    public string UserId { get; set; } = null!;

    public EAuditAction Action { get; set; }

    public EAuditDocType DocType { get; set; }
    public string? DocId { get; set; }
    public string? DocCode { get; set; }

    public string? Title { get; set; }
    public string? Content { get; set; }

    public DateTimeOffset Time { get; set; } = DateTimeOffset.UtcNow;

    public virtual User? User { get; set; }
}

internal class UserAuditConfig : IEntityTypeConfiguration<UserAudit> {

    public void Configure(EntityTypeBuilder<UserAudit> builder) {
        builder.ToTable(nameof(UserAudit));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);
        builder.Property(o => o.MerchantId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.UserId).HasMaxLength(32).IsRequired();

        // index
        builder.HasIndex(o => o.MerchantId);
        builder.HasIndex(o => new { o.MerchantId, o.UserId });

        builder.Property(o => o.Time).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();

        // fk
        builder.HasOne(o => o.User).WithMany(o => o.UserAudits).HasForeignKey(o => o.UserId);
    }
}
