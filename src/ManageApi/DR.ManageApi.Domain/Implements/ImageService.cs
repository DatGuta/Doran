using FMS.Common.Exceptions;
using FMS.Constant.Enums;
using FMS.Database.Models;
using FMS.Helper;
using FMS.ManageApi.Domain.Services.Interfaces;
using FMS.Message;
using FMS.Models.Dto;
using Microsoft.Extensions.Configuration;

namespace FMS.ManageApi.Domain.Services.Implements;

public class ImageService(IServiceProvider serviceProvider) : IImageService {
    private readonly IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
    private readonly FmsContext db = serviceProvider.GetRequiredService<FmsContext>();

    public async Task<List<ItemImage>> List(string merchantId, EItemImage itemType, string itemId, bool withNoTracking = true) {
        return await this.List(merchantId, itemType, new List<string> { itemId }, withNoTracking);
    }

    public async Task<List<ItemImage>> List(string merchantId, EItemImage itemType, List<string> itemIds, bool withNoTracking = true) {
        var query = this.db.ItemImages.Where(o => o.MerchantId == merchantId && o.ItemType == itemType && itemIds.Contains(o.ItemId));
        if (withNoTracking) query = query.AsNoTracking();
        return await query.ToListAsync();
    }

    public async Task Save(string merchantId, EItemImage itemType, string itemId, ImageDto model, ItemImage? entity = null) {
        if (model.Data == null || model.Data.Length == 0)
            return;

        if (string.IsNullOrWhiteSpace(model.Id)) {
            model.Id = NGuidHelper.New();
            await Create(merchantId, itemType, itemId, model);
            return;
        }
        await this.Update(model, entity);
    }

    public async Task Delete(string id, ItemImage? entity) {
        entity ??= await this.db.ItemImages.FirstOrDefaultAsync(o => o.Id == id);
        ManagedException.ThrowIf(entity == null, Messages.Image.Image_Error);

        await FtpHelper.DeleteFile(entity!.Image, this.configuration);

        this.db.ItemImages.Remove(entity);
        await this.db.SaveChangesAsync();
    }

    private async Task Create(string merchantId, EItemImage itemType, string itemId, ImageDto model) {
        ItemImage entity = new() {
            Id = NGuidHelper.New(model.Id),
            MerchantId = merchantId,
            ItemId = itemId,
            ItemType = itemType,
            Name = model.Name,
        };
        entity.Image = await this.UploadImage(entity, model.Data);

        await this.db.AddAsync(entity);
        await this.db.SaveChangesAsync();
    }

    private async Task Update(ImageDto model, ItemImage? entity) {
        entity ??= await this.db.ItemImages.FirstOrDefaultAsync(o => o.Id == model.Id);
        ManagedException.ThrowIfNull(entity, Messages.Image.Image_Error);

        entity.Name = model.Name;
        entity.Image = await this.UploadImage(entity, model.Data);

        this.db.ItemImages.Update(entity);
        await this.db.SaveChangesAsync();
    }

    private async Task<string> UploadImage(ItemImage item, byte[]? data) {
        var (directories, filename) = GetFilePath(EFile.Images, item);
        await FtpHelper.UploadImage(directories, filename, data, this.configuration);
        return filename;
    }

    private static (string[], string) GetFilePath(EFile file, ItemImage item) {
        string fileType = Enum.IsDefined(file) ? file.ToString().ToLower() : "temporary";
        string imageType = Enum.IsDefined(item.ItemType) ? item.ItemType.ToString().ToLower() : "other";
        string extentions = Path.GetExtension(item.Name);

        string[] directories = new string[] {
                $"{fileType}/{item.MerchantId}",
                $"{fileType}/{item.MerchantId}/{imageType}"
            };

        return (directories, $"{fileType}/{item.MerchantId}/{imageType}/{item.Id}{extentions}");
    }
}
