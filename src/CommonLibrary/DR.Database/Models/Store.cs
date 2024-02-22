namespace DR.Database.Models;

public partial class Store : ISyncEntity {
    public string Id { get; set; } = null!;
    public string MerchantId { get; set; } = null!;
    public string? WarehouseId { get; set; }

    [Description("Mã cửa hàng")]
    public string Code { get; set; } = null!;

    [Description("Tên cửa hàng")]
    public string Name { get; set; } = null!;

    public string SearchName { get; set; } = null!;

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

    [Description("Cho phép bán")]
    public bool IsActive { get; set; }

    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ModifiedDate { get; set; } = DateTimeOffset.UtcNow;
    public bool IsDelete { get; set; }

    public virtual Warehouse? Warehouse { get; set; }
    public virtual ICollection<Order>? Orders { get; set; }
    public virtual ICollection<ProductOnStore>? ProductOnStores { get; set; }
}
