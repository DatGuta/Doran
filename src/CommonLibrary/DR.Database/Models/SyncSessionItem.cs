namespace DR.Database.Models;

public class SyncSessionItem {
    public string Id { get; set; } = null!;
    public string SyncSessionId { get; set; } = null!;
    public string RecordType { get; set; } = string.Empty;
    public string RecordId { get; set; } = string.Empty;
    public int RecordOrder { get; set; }

    public virtual SyncSession? SyncSession { get; set; }
}

internal class SyncSessionItemConfig : IEntityTypeConfiguration<SyncSessionItem> {

    public void Configure(EntityTypeBuilder<SyncSessionItem> builder) {
        builder.ToTable(nameof(SyncSessionItem));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);
        builder.Property(o => o.SyncSessionId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.RecordType).HasMaxLength(50).IsRequired();
        builder.Property(o => o.RecordId).HasMaxLength(32).IsRequired();

        // fk
        builder.HasOne(o => o.SyncSession).WithMany(o => o.SyncSessionItems).HasForeignKey(o => o.SyncSessionId);
    }
}
