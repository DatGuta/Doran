using FMS.Constant.Enums;
using FMS.Database.Models;
using FMS.Helper;
using FMS.ManageApi.Domain.Services.Interfaces;

namespace FMS.ManageApi.Domain.Services.Implements;

public class MerchantItemDefaultService(IServiceProvider serviceProvider) : IMerchantItemDefaultService {
    private readonly FmsContext db = serviceProvider.GetRequiredService<FmsContext>();

    public async Task<string?> Get(string merchantId, EMerchantItemDefault itemType, CancellationToken cancellationToken = default) {
        return await this.db.MerchantItemDefaults.AsNoTracking()
            .Where(o => o.MerchantId == merchantId && o.Type == itemType).Select(o => o.ItemId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Dictionary<EMerchantItemDefault, string?>> List(string merchantId, List<EMerchantItemDefault>? types = null, CancellationToken cancellationToken = default) {
        if (types == null || types.Count == 0) {
            types = [.. Enum.GetValues<EMerchantItemDefault>()];
        }
        return await this.db.MerchantItemDefaults.AsNoTracking()
            .Where(o => o.MerchantId == merchantId && types.Contains(o.Type))
            .ToDictionaryAsync(k => k.Type, v => v.ItemId, cancellationToken);
    }

    public async Task Save(string merchantId, Dictionary<EMerchantItemDefault, string?> items, CancellationToken cancellationToken = default) {
        var settings = await this.db.MerchantItemDefaults
            .Where(o => o.MerchantId == merchantId && items.Keys.Contains(o.Type))
            .ToListAsync(cancellationToken);

        var keys = settings.Select(o => o.Type).ToList();
        var addKeys = items.Where(o => !keys.Contains(o.Key)).ToList();
        if (addKeys.Count > 0) {
            var addItems = items.Select(o => new MerchantItemDefault {
                Id = NGuidHelper.New(),
                ItemId = o.Value,
                MerchantId = merchantId,
                Type = o.Key,
            });
            this.db.MerchantItemDefaults.AddRange(addItems);
        }
        settings.ForEach(o => o.ItemId = items.GetValueOrDefault(o.Type));
        await this.db.SaveChangesAsync(cancellationToken);
    }
}
