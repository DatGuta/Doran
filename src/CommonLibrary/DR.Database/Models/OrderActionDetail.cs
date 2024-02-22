namespace DR.Database.Models;

public partial class OrderActionDetail {
    public string Id { get; set; } = null!;
    public string OrderActionId { get; set; } = null!;
    public string OrderDetailId { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }

    public virtual OrderAction? OrderAction { get; set; }
    public virtual OrderDetail? OrderDetail { get; set; }
}

internal class OrderActionDetailConfig : IEntityTypeConfiguration<OrderActionDetail> {

    public void Configure(EntityTypeBuilder<OrderActionDetail> builder) {
        builder.ToTable(nameof(OrderActionDetail));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);

        builder.Property(o => o.OrderActionId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.OrderDetailId).HasMaxLength(32).IsRequired();

        // index
        builder.HasIndex(o => o.OrderActionId);
        builder.HasIndex(o => o.OrderDetailId);
        builder.HasIndex(o => new { o.OrderActionId, o.OrderDetailId }).IsUnique();

        // fk
        builder.HasOne(o => o.OrderAction).WithMany(o => o.OrderActionDetails).HasForeignKey(o => o.OrderActionId);
        builder.HasOne(o => o.OrderDetail).WithMany(o => o.OrderActionDetails).HasForeignKey(o => o.OrderDetailId);
    }
}
