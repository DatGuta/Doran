﻿namespace DR.Redis;

public static class RedisKey {

    public static string GetSuggestBestSellingKey(string merchantId, string warehouseId, string storeId) {
        return $"Merchant:{merchantId}:SuggestBestSelling:WarehouseId:{warehouseId}:StoreId:{storeId}";
    }

    public static string GetGlobalSettingKey(string merchantId) {
        return $"Merchant:{merchantId}:GlobalSetting";
    }

    public static string GetSessionKey(string source, string userId) {
        return $"Session:{source}:{userId}";
    }
}
