namespace DR.Database.Models;

public partial class Product : ISyncEntity {
    public string Id { get; set; } = null!;
    public string MerchantId { get; set; } = null!;

    [Description("Mã sản phẩm")]
    public string Code { get; set; } = null!;

    [Description("Tên sản phẩm")]
    public string Name { get; set; } = null!;

    public string DisplayName { get; set; } = string.Empty;

    public string SearchName { get; set; } = null!;

    [Description("Mô tả")]
    public string? Description { get; set; }

    [Description("NPK")]
    public bool IsNPK { get; set; }

    [Description("Khuyến mãi")]
    public bool IsPromotion { get; set; }

    [Description("Loại NPK")]
    public string? NPKType { get; set; }

    [Description("Trọng lượng")]
    public decimal NetWeight { get; set; }

    [Description("Ngày tạo")]
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset ModifiedDate { get; set; } = DateTimeOffset.UtcNow;

    public bool IsDelete { get; set; }

    public virtual ICollection<OrderDetail>? OrderDetails { get; set; }

    [Description("Danh mục")]
    public virtual ICollection<ProductCategory>? ProductCategories { get; set; }

    [Description("Thương hiệu")]
    public virtual ICollection<ProductBrand>? ProductBrands { get; set; }

    [Description("Cửa hàng được bán")]
    public virtual ICollection<ProductOnStore>? ProductOnStores { get; set; }

    [Description("Danh sách tồn kho")]
    public virtual ICollection<ProductOnWarehouse>? ProductOnWarehouses { get; set; }

    public virtual ICollection<ProductTracking>? ProductTrackings { get; set; }
    public virtual ICollection<WarehouseImportDetail>? WarehouseImportDetails { get; set; }
    public virtual ICollection<WarehouseExportDetail>? WarehouseExportDetails { get; set; }
    public virtual ICollection<WarehouseExportOtherDetail>? WarehouseExportOtherDetails { get; set; }
    public virtual ICollection<WarehouseTransferDetail>? WarehouseTransferDetails { get; set; }
    public virtual ICollection<WarehouseRefundDetail>? WarehouseRefundDetails { get; set; }
    public virtual ICollection<WarehouseAdjustmentDetail>? WarehouseAdjustmentDetails { get; set; }
}

public partial class Product {

    public void UpdateFrom(Product entity) {
        this.Name = entity.Name;
        this.DisplayName = entity.DisplayName;
        this.SearchName = entity.SearchName;
        this.Description = entity.Description;
        this.IsNPK = entity.IsNPK;
        this.IsPromotion = entity.IsPromotion;
        this.NPKType = entity.NPKType;
        this.NetWeight = entity.NetWeight;
        this.ModifiedDate = DateTimeOffset.UtcNow;

        this.UpdateProductBrand(entity.ProductBrands);
        this.UpdateProductCategory(entity.ProductCategories);
        this.UpdateProductOnWarehouse(entity.ProductOnWarehouses);
        this.UpdateProductOnStore(entity.ProductOnStores);
    }

    private void UpdateProductBrand(ICollection<ProductBrand>? productBrands) {
        this.ProductBrands ??= new List<ProductBrand>();
        var brandIds = productBrands?.Select(o => o.BrandId).ToList() ?? new List<string>();

        foreach (var item in this.ProductBrands) {
            if (brandIds.Contains(item.BrandId)) {
                brandIds.Remove(item.BrandId);
                item.IsDelete = false;
            } else {
                item.IsDelete = true;
            }
        }

        foreach (var brandId in brandIds) {
            this.ProductBrands.Add(new ProductBrand {
                Id = Guid.NewGuid().ToString("N"),
                MerchantId = this.MerchantId,
                ProductId = this.Id,
                BrandId = brandId,
            });
        }
    }

    private void UpdateProductCategory(ICollection<ProductCategory>? productCategories) {
        this.ProductCategories ??= new List<ProductCategory>();
        var categoryIds = productCategories?.Select(o => o.CategoryId).ToList() ?? new List<string>();

        foreach (var item in this.ProductCategories) {
            if (categoryIds.Contains(item.CategoryId)) {
                categoryIds.Remove(item.CategoryId);
                item.IsDelete = false;
            } else {
                item.IsDelete = true;
            }
        }

        foreach (var categoryId in categoryIds) {
            this.ProductCategories.Add(new ProductCategory {
                Id = Guid.NewGuid().ToString("N"),
                MerchantId = this.MerchantId,
                ProductId = this.Id,
                CategoryId = categoryId,
            });
        }
    }

    private void UpdateProductOnWarehouse(ICollection<ProductOnWarehouse>? productOnWarehouses) {
        this.ProductOnWarehouses ??= new List<ProductOnWarehouse>();
        var dbWarehouseIds = this.ProductOnWarehouses.Select(o => o.WarehouseId).ToList();
        var modelWarehouseIds = productOnWarehouses?.Select(o => o.WarehouseId!).ToList() ?? new List<string>();
        var warehouseIds = dbWarehouseIds.Concat(modelWarehouseIds).ToList();
        foreach (var warehouseId in warehouseIds) {
            var productOnWarehouse = productOnWarehouses?.FirstOrDefault(o => o.WarehouseId! == warehouseId);
            var productOnWarehouseDb = this.ProductOnWarehouses.FirstOrDefault(o => o.WarehouseId == warehouseId);
            if (productOnWarehouseDb != null) {
                productOnWarehouseDb.OnHand = productOnWarehouse?.OnHand ?? default;
                productOnWarehouseDb.IsActive = productOnWarehouse?.IsActive ?? default;
                productOnWarehouseDb.ModifiedDate = DateTimeOffset.UtcNow;
            }
            if (productOnWarehouseDb == null && productOnWarehouse != null) {
                this.ProductOnWarehouses.Add(new ProductOnWarehouse {
                    Id = Guid.NewGuid().ToString("N"),
                    MerchantId = this.MerchantId,
                    WarehouseId = warehouseId,
                    ProductId = this.Id,
                    OnHand = productOnWarehouse.OnHand,
                    IsActive = productOnWarehouse.IsActive,
                    ModifiedDate = DateTimeOffset.UtcNow,
                });
            }
        }
    }

    private void UpdateProductOnStore(ICollection<ProductOnStore>? productOnStores) {
        this.ProductOnStores ??= new List<ProductOnStore>();
        if (productOnStores == null || productOnStores.Count == 0) {
            return;
        }

        var currentStoreIds = productOnStores.Select(o => o.StoreId).Distinct().ToList();
        var existedStoreIds = this.ProductOnStores.Select(o => o.StoreId).ToList();

        var newStoreIds = currentStoreIds.Except(existedStoreIds).ToList();
        var deleteStoreIds = existedStoreIds.Except(currentStoreIds).ToList();

        foreach (var item in this.ProductOnStores) {
            item.IsDelete = deleteStoreIds.Contains(item.StoreId);
        }

        foreach (var storeId in newStoreIds) {
            this.ProductOnStores.Add(new ProductOnStore {
                Id = Guid.NewGuid().ToString("N"),
                MerchantId = this.MerchantId,
                StoreId = storeId,
                ProductId = this.Id,
            });
        }
    }
}

internal class ProductConfig : IEntityTypeConfiguration<Product> {

    public void Configure(EntityTypeBuilder<Product> builder) {
        builder.ToTable(nameof(Product));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);
        builder.Property(o => o.MerchantId).HasMaxLength(32).IsRequired();

        builder.Property(o => o.Code).HasMaxLength(50).IsRequired();
        builder.Property(o => o.Name).HasMaxLength(255).IsRequired();
        builder.Property(o => o.SearchName).HasMaxLength(255).IsRequired();
        builder.Property(o => o.DisplayName).HasMaxLength(255).IsRequired();

        builder.Property(o => o.Description).HasMaxLength(2000);

        builder.Property(o => o.NPKType).HasMaxLength(255);

        builder.Property(o => o.NetWeight).IsRequired();

        builder.Property(o => o.CreatedDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o));
        builder.Property(o => o.ModifiedDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o));

        // index
        builder.HasIndex(o => o.MerchantId);
        builder.HasIndex(o => new { o.MerchantId, o.Code }).IsUnique();

        // fk
        builder.HasMany(o => o.OrderDetails).WithOne(o => o.Product).HasForeignKey(o => o.ProductId);
        builder.HasMany(o => o.ProductOnWarehouses).WithOne(o => o.Product).HasForeignKey(o => o.ProductId);
        builder.HasMany(o => o.ProductTrackings).WithOne(o => o.Product).HasForeignKey(o => o.ProductId);
        builder.HasMany(o => o.WarehouseImportDetails).WithOne(o => o.Product).HasForeignKey(o => o.ProductId);
        builder.HasMany(o => o.WarehouseExportDetails).WithOne(o => o.Product).HasForeignKey(o => o.ProductId);
        builder.HasMany(o => o.WarehouseRefundDetails).WithOne(o => o.Product).HasForeignKey(o => o.ProductId);
        builder.HasMany(o => o.WarehouseTransferDetails).WithOne(o => o.Product).HasForeignKey(o => o.ProductId);
        builder.HasMany(o => o.WarehouseAdjustmentDetails).WithOne(o => o.Product).HasForeignKey(o => o.ProductId);
        builder.HasMany(o => o.ProductOnStores).WithOne(o => o.Product).HasForeignKey(o => o.ProductId);
        builder.HasMany(o => o.ProductCategories).WithOne(o => o.Product).HasForeignKey(o => o.ProductId);
    }
}
