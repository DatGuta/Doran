namespace DR.Database.Models;

public partial class ReceiptDetail {
    public string Id { get; set; } = null!;
    public string ReceiptId { get; set; } = null!;
    public string OrderId { get; set; } = null!;
    public decimal Value { get; set; }

    public virtual Receipt? Receipt { get; set; }
    public virtual Order? Order { get; set; }
}

internal class ReceiptDetailConfig : IEntityTypeConfiguration<ReceiptDetail> {

    public void Configure(EntityTypeBuilder<ReceiptDetail> builder) {
        builder.ToTable(nameof(ReceiptDetail));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);

        builder.Property(o => o.ReceiptId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.OrderId).HasMaxLength(32).IsRequired();

        // index
        builder.HasIndex(o => o.OrderId);

        // fk
        builder.HasOne(o => o.Receipt).WithMany(o => o.ReceiptDetails).HasForeignKey(o => o.ReceiptId);
        builder.HasOne(o => o.Order).WithMany(o => o.ReceiptDetails).HasForeignKey(o => o.OrderId);
    }
}
