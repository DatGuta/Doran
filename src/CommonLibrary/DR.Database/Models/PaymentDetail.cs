namespace DR.Database.Models;

public partial class PaymentDetail {
    public string Id { get; set; } = null!;
    public string PaymentId { get; set; } = null!;

    public string? OrderActionId { get; set; }
    public string? OrderId { get; set; }
    public decimal Value { get; set; }

    public virtual Payment? Payment { get; set; }
    public virtual Order? Order { get; set; }
}

public class PaymentDetailConfig : IEntityTypeConfiguration<PaymentDetail> {

    public void Configure(EntityTypeBuilder<PaymentDetail> builder) {
        builder.ToTable(nameof(PaymentDetail));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);

        builder.Property(o => o.PaymentId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.OrderActionId).HasMaxLength(32);
        builder.Property(o => o.OrderId).HasMaxLength(32);

        // index
        builder.HasIndex(o => o.OrderActionId);
        builder.HasIndex(o => o.OrderId);

        // fk
        builder.HasOne(o => o.Payment).WithMany(o => o.PaymentDetails).HasForeignKey(o => o.PaymentId);
        builder.HasOne(o => o.Order).WithMany(o => o.PaymentDetails).HasForeignKey(o => o.OrderId);
    }
}
