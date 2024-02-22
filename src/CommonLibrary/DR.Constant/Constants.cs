﻿using DR.Constant.Enums;

namespace DR.Constant;

public static class Constants {
    public const string TokenMerchantId = "MerchantId";
    public const string TokenUserId = "UserId";
    public const string TokenRefreshToken = "RefreshToken";
    public const string TokenSource = "Source";
    public const string TokenSession = "Session";

    public const string HeaderApiKey = "ApiKey";
    public const string HeaderApiSecret = "ApiSecret";

    public const string HeaderStoreId = "StoreId";
    public const string HeaderWarehouseId = "WarehouseId";

    public static IReadOnlyDictionary<EWarehouse, EOrder> OrderTypeMap { get; } = new Dictionary<EWarehouse, EOrder>() {
        { EWarehouse.Normal, EOrder.Normal },
        { EWarehouse.Ticket, EOrder.Ticket },
    };

}
