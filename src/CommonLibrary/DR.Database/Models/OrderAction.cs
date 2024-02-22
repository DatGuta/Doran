using DR.Constant.Enums;

namespace DR.Database.Models;

public partial class OrderAction {
    public string Id { get; set; } = null!;
    public string MerchantId { get; set; } = null!;
    public string OrderId { get; set; } = null!;
    public EOrderAction Type { get; set; } = default!;

    [Description("Mã phiếu")]
    public string Code { get; set; } = null!;

    [Description("Số lần xuất")]
    public int Time { get; set; }

    [Description("Ngày xuất")]
    public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;

    public bool IsDelete { get; set; }

    public virtual Order? Order { get; set; }
    public virtual ICollection<OrderActionDetail>? OrderActionDetails { get; set; }
}

internal class OrderActionConfig : IEntityTypeConfiguration<OrderAction> {

    public void Configure(EntityTypeBuilder<OrderAction> builder) {
        builder.ToTable(nameof(OrderAction));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);
        builder.Property(o => o.MerchantId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.OrderId).HasMaxLength(32).IsRequired();

        builder.Property(o => o.Code).HasMaxLength(50).IsRequired();

        builder.Property(o => o.Date).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();

        // index
        builder.HasIndex(o => o.MerchantId);
        builder.HasIndex(o => new { o.MerchantId, o.OrderId });
        builder.HasIndex(o => new { o.MerchantId, o.Type });
        builder.HasIndex(o => new { o.MerchantId, o.OrderId, o.Type });
        builder.HasIndex(o => new { o.MerchantId, o.OrderId, o.Type, o.Time });

        // fk
        builder.HasOne(o => o.Order).WithMany(o => o.OrderActions).HasForeignKey(o => o.OrderId);

        builder.HasMany(o => o.OrderActionDetails).WithOne(o => o.OrderAction).HasForeignKey(o => o.OrderActionId);
    }
}
