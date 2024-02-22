using DR.Constant.Enums;

namespace DR.Database.Models;

public partial class SyncSession : IEntity {
    public string Id { get; set; } = null!;
    public string MerchantId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string DeviceName { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public DateTimeOffset From { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset To { get; set; } = DateTimeOffset.UtcNow;
    public ESyncSession Status { get; set; }
    public int BatchRead { get; set; }
    public int CurrentStep { get; set; }
    public int TotalStep { get; set; }
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

    public virtual ICollection<SyncSessionItem>? SyncSessionItems { get; set; }
}

internal class SyncSessionConfig : IEntityTypeConfiguration<SyncSession> {

    public void Configure(EntityTypeBuilder<SyncSession> builder) {
        builder.ToTable(nameof(SyncSession));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);
        builder.Property(o => o.MerchantId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.UserId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.DeviceName).HasMaxLength(500).IsRequired();
        builder.Property(o => o.IpAddress).HasMaxLength(20).IsRequired();

        builder.Property(o => o.From).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o));
        builder.Property(o => o.To).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o));
        builder.Property(o => o.CreatedDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o));

        // index
        builder.HasIndex(o => o.MerchantId);

        // fk
        builder.HasMany(o => o.SyncSessionItems).WithOne(o => o.SyncSession).HasForeignKey(o => o.SyncSessionId);
    }
}
