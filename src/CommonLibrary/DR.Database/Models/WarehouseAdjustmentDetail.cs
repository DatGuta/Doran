namespace DR.Database.Models;

public partial class WarehouseAdjustmentDetail {
    public string Id { get; set; } = null!;
    public string WarehouseAdjustmentId { get; set; } = null!;

    [Description("Sản phẩm")]
    public string ProductId { get; set; } = null!;

    [Description("Tồn kho")]
    public decimal OnHand { get; set; }

    [Description("Thực tế")]
    public decimal Actual { get; set; }

    [Description("Chênh lệch")]
    public decimal Difference { get; set; }

    [Description("Ghi chú")]
    public string? Description { get; set; }

    public virtual WarehouseAdjustment? WarehouseAdjustment { get; set; }
    public virtual Product? Product { get; set; }
}

internal class WarehouseAdjustmentDetailConfig : IEntityTypeConfiguration<WarehouseAdjustmentDetail> {

    public void Configure(EntityTypeBuilder<WarehouseAdjustmentDetail> builder) {
        builder.ToTable(nameof(WarehouseAdjustmentDetail));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32).IsRequired();
        builder.Property(o => o.WarehouseAdjustmentId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.ProductId).HasMaxLength(32).IsRequired();

        // index
        builder.HasIndex(o => o.WarehouseAdjustmentId);

        // fk
        builder.HasOne(o => o.WarehouseAdjustment).WithMany(o => o.WarehouseAdjustmentDetails).HasForeignKey(o => o.WarehouseAdjustmentId);
        builder.HasOne(o => o.Product).WithMany(o => o.WarehouseAdjustmentDetails).HasForeignKey(o => o.ProductId);
    }
}
