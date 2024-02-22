using DR.Constant.Enums;
using DR.Database.Models;
using DR.Models.Dto;

namespace DR.ManageApi.Domain.Services.Interfaces;

public interface IImageService {

    Task<List<ItemImage>> List(string merchantId, EItemImage itemType, string itemId, bool withNoTracking = true);

    Task<List<ItemImage>> List(string merchantId, EItemImage itemType, List<string> itemIds, bool withNoTracking = true);

    Task Save(string merchantId, EItemImage itemType, string itemId, ImageDto model, ItemImage? entity = null);

    Task Delete(string id, ItemImage? entity);
}
