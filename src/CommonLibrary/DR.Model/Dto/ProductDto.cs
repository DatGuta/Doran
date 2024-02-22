using Newtonsoft.Json;

namespace DR.Models.Dto {

    public class ProductDto {
        public string? Id { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; } = null!;
        public string DisplayName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsNPK { get; set; }
        public string? NPKType { get; set; }
        public decimal NetWeight { get; set; }
        public bool IsPromotion { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public List<ImageDto>? Images { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public List<ProductOnWasehouseDto>? ProductOnWasehouses { get; set; }

        public List<string> StoreIds { get; set; } = new();
        public List<string> CategoryIds { get; set; } = new();
        public List<string> BrandIds { get; set; } = new();

        public Product ToEntity(string merchantId) {
            string id = NGuidHelper.New(Id);
            return new Product() {
                Id = id,
                MerchantId = merchantId,
                Code = Code!,
                Name = Name,
                DisplayName = DisplayName,
                SearchName = StringHelper.UnsignedUnicode(Name),
                Description = Description,
                IsNPK = IsNPK,
                NPKType = NPKType,
                NetWeight = NetWeight,
                IsPromotion = IsPromotion,
                ProductOnWarehouses = ProductOnWasehouses?.Select(o => new ProductOnWarehouse {
                    Id = NGuidHelper.New(),
                    MerchantId = merchantId,
                    WarehouseId = o.Warehouse!.Id!,
                    ProductId = id,
                    OnHand = o.OnHand,
                    IsActive = o.IsActive,
                }).ToList(),
                ProductOnStores = StoreIds?.Select(storeId => new ProductOnStore {
                    Id = NGuidHelper.New(),
                    MerchantId = merchantId,
                    StoreId = storeId,
                    ProductId = id,
                }).ToList(),
                ProductCategories = CategoryIds?.Select(categoryId => new ProductCategory {
                    Id = NGuidHelper.New(),
                    MerchantId = merchantId,
                    CategoryId = categoryId,
                    ProductId = id,
                }).ToList(),
                ProductBrands = BrandIds?.Select(brandId => new ProductBrand {
                    Id = NGuidHelper.New(),
                    MerchantId = merchantId,
                    BrandId = brandId,
                    ProductId = id,
                }).ToList(),
            };
        }

        [return: NotNullIfNotNull(nameof(entity))]
        public static ProductDto? FromEntity(Product? entity,
            List<ProductOnWarehouse>? productOnWarehouses = null,
            List<ItemImage>? itemImageEntities = null,
            string? currentUrl = null,
            bool isWebSale = false) {
            if (entity == null) return default;

            return new ProductDto {
                Id = entity.Id,
                Code = entity.Code,
                Name = isWebSale ? entity.DisplayName : entity.Name,
                DisplayName = entity.DisplayName,
                Description = entity.Description,
                IsNPK = entity.IsNPK,
                NPKType = entity.NPKType,
                NetWeight = entity.NetWeight,
                IsPromotion = entity.IsPromotion,
                Images = itemImageEntities?.Select(o => ImageDto.FromEntity(o, currentUrl)!).ToList(),
                ProductOnWasehouses = productOnWarehouses?.Select(o => ProductOnWasehouseDto.FromEntity(o)!).ToList(),
            };
        }
    }
}
