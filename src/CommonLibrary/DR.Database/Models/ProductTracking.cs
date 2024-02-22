using DR.Constant.Enums;

namespace DR.Database.Models;

public partial class ProductTracking : IEntity {
    public string Id { get; set; } = null!;

    public string MerchantId { get; set; } = null!;
    public string WarehouseId { get; set; } = null!;
    public string ProductId { get; set; } = null!;

    public EProductDocumentType DocumentType { get; set; }
    public string DocumentId { get; set; } = null!;
    public string DocumentCode { get; set; } = null!;

    public decimal OnHandBefore { get; set; }
    public decimal Quantity { get; set; }
    public decimal OnHandAfter { get; set; }

    public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;
    public bool IsUpdateOnHand { get; set; }
    public bool IsDelete { get; set; }

    public string TableName { get; set; } = string.Empty;
    public string ItemId { get; set; } = string.Empty;

    public virtual Warehouse? Warehouse { get; set; }
    public virtual Product? Product { get; set; }
}

internal class ProductTrackingConfig : IEntityTypeConfiguration<ProductTracking> {

    public void Configure(EntityTypeBuilder<ProductTracking> builder) {
        builder.ToTable(nameof(ProductTracking));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);
        builder.Property(o => o.MerchantId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.WarehouseId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.ProductId).HasMaxLength(32).IsRequired();

        builder.Property(o => o.DocumentType).HasMaxLength(50).IsRequired();
        builder.Property(o => o.DocumentId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.DocumentCode).HasMaxLength(255).IsRequired();

        builder.Property(o => o.TableName).HasMaxLength(50).HasDefaultValue("");
        builder.Property(o => o.ItemId).HasMaxLength(32).HasDefaultValue("");

        builder.Property(o => o.Date).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();

        // index
        builder.HasIndex(o => o.MerchantId);
        builder.HasIndex(o => o.IsUpdateOnHand);
        builder.HasIndex(o => new { o.MerchantId, o.WarehouseId, o.ProductId });
        builder.HasIndex(o => new { o.MerchantId, o.WarehouseId, o.ProductId, o.IsUpdateOnHand });
        builder.HasIndex(o => new { o.MerchantId, o.WarehouseId, o.ProductId, o.IsUpdateOnHand, o.IsDelete });
        builder.HasIndex(o => new { o.MerchantId, o.WarehouseId, o.DocumentType, o.DocumentId });

        // fk
        builder.HasOne(o => o.Product).WithMany(o => o.ProductTrackings).HasForeignKey(o => o.ProductId);
        builder.HasOne(o => o.Warehouse).WithMany(o => o.ProductTrackings).HasForeignKey(o => o.WarehouseId);
    }
}
