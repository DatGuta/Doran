using DR.Constant.Enums;

namespace DR.Database.Models;

public partial class Warehouse : ISyncEntity {
    public string Id { get; set; } = null!;
    public string MerchantId { get; set; } = null!;

    [Description("Mã kho")]
    public string Code { get; set; } = null!;

    [Description("Tên kho")]
    public string Name { get; set; } = null!;

    public string SearchName { get; set; } = null!;

    [Description("Loại")]
    public EWarehouse Type { get; set; }

    [Description("Tỉnh/Thành phố")]
    public string? Province { get; set; }

    [Description("Quận/Huyện")]
    public string? District { get; set; }

    [Description("Phường/Xã")]
    public string? Commune { get; set; }

    [Description("Địa chỉ")]
    public string? Address { get; set; }

    [Description("Số điện thoại")]
    public string? Phone { get; set; }

    [Description("Email")]
    public string? Email { get; set; }

    [Description("Cho phép hoạt động")]
    public bool IsActive { get; set; }

    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ModifiedDate { get; set; } = DateTimeOffset.UtcNow;
    public bool IsDelete { get; set; }

    public virtual ICollection<Store>? Stores { get; set; }
    public virtual ICollection<Order>? Orders { get; set; }
    public virtual ICollection<ProductOnWarehouse>? ProductOnWarehouses { get; set; }
    public virtual ICollection<ProductTracking>? ProductTrackings { get; set; }
    public virtual ICollection<WarehouseImport>? WarehouseImports { get; set; }
    public virtual ICollection<WarehouseExport>? WarehouseExports { get; set; }
    public virtual ICollection<WarehouseExportOther>? WarehouseExportOthers { get; set; }
    public virtual ICollection<WarehouseTransfer>? FromWarehouseTransfers { get; set; }
    public virtual ICollection<WarehouseTransfer>? ToWarehouseTransfers { get; set; }
    public virtual ICollection<WarehouseRefund>? WarehouseRefunds { get; set; }
    public virtual ICollection<WarehouseAdjustment>? WarehouseAdjustments { get; set; }
}

public partial class Warehouse {

    public void UpdateFrom(Warehouse entity) {
        this.Name = entity.Name;
        this.SearchName = entity.SearchName;
        this.Phone = entity.Phone;
        this.Email = entity.Email;
        this.Province = entity.Province;
        this.District = entity.District;
        this.Commune = entity.Commune;
        this.Address = entity.Address;
        this.IsActive = entity.IsActive;
        this.ModifiedDate = DateTimeOffset.UtcNow;
    }
}

internal class WarehouseConfig : IEntityTypeConfiguration<Warehouse> {

    public void Configure(EntityTypeBuilder<Warehouse> builder) {
        builder.ToTable(nameof(Warehouse));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);
        builder.Property(o => o.MerchantId).HasMaxLength(32).IsRequired();

        builder.Property(o => o.Code).HasMaxLength(50).IsRequired();
        builder.Property(o => o.Name).HasMaxLength(255).IsRequired();
        builder.Property(o => o.SearchName).HasMaxLength(255).IsRequired();

        builder.Property(o => o.Province).HasMaxLength(20);
        builder.Property(o => o.District).HasMaxLength(20);
        builder.Property(o => o.Commune).HasMaxLength(20);
        builder.Property(o => o.Address).HasMaxLength(255);

        builder.Property(o => o.CreatedDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o));
        builder.Property(o => o.ModifiedDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o));

        // index
        builder.HasIndex(o => o.MerchantId);
        builder.HasIndex(o => new { o.MerchantId, o.Code }).IsUnique();

        // fk
        builder.HasMany(o => o.Stores).WithOne(o => o.Warehouse).HasForeignKey(o => o.WarehouseId);
        builder.HasMany(o => o.Orders).WithOne(o => o.Warehouse).HasForeignKey(o => o.WarehouseId);
        builder.HasMany(o => o.ProductOnWarehouses).WithOne(o => o.Warehouse).HasForeignKey(o => o.WarehouseId);
        builder.HasMany(o => o.ProductTrackings).WithOne(o => o.Warehouse).HasForeignKey(o => o.WarehouseId);
        builder.HasMany(o => o.WarehouseImports).WithOne(o => o.Warehouse).HasForeignKey(o => o.WarehouseId);
        builder.HasMany(o => o.WarehouseExports).WithOne(o => o.Warehouse).HasForeignKey(o => o.WarehouseId);
        builder.HasMany(o => o.WarehouseExportOthers).WithOne(o => o.Warehouse).HasForeignKey(o => o.WarehouseId);
        builder.HasMany(o => o.WarehouseRefunds).WithOne(o => o.Warehouse).HasForeignKey(o => o.WarehouseId);
        builder.HasMany(o => o.ToWarehouseTransfers).WithOne(o => o.ToWarehouse).HasForeignKey(o => o.ToWarehouseId);
        builder.HasMany(o => o.FromWarehouseTransfers).WithOne(o => o.FromWarehouse).HasForeignKey(o => o.FromWarehouseId);
        builder.HasMany(o => o.WarehouseAdjustments).WithOne(o => o.Warehouse).HasForeignKey(o => o.WarehouseId);
    }
}
