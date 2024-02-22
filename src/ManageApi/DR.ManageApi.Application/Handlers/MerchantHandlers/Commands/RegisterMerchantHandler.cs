using DR.Common.Exceptions;
using DR.Common.Hashers;
using DR.Constant.Enums;
using DR.Database.Models;
using DR.Helper;
using DR.ManageApi.Domain.Actions.AutoGenerate;
using DR.ManageApi.Domain.Services.Interfaces;
using DR.Message;

namespace FMS.ManageApi.Application.Handlers.MerchantHandlers.Commands;

public class RegisterMerchantCommand : IRequest {
    public RegisterMerchant Merchant { get; set; } = new();
    public RegisterAdminUser User { get; set; } = new();
    public RegisterStore? Store { get; set; }
    public RegisterWarehouse? Warehouse { get; set; }
}

public class RegisterMerchant {
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DateTimeOffset ExpiredDate { get; set; }
}

public class RegisterAdminUser {
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string PinCode { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Phone { get; set; }
    public Resource.Unit? Province { get; set; }
    public Resource.Unit? District { get; set; }
    public Resource.Unit? Commune { get; set; }
    public string? Address { get; set; }
}

public class RegisterStore {
    public string? Code { get; set; }
    public string Name { get; set; } = null!;
    public Resource.Unit? Province { get; set; }
    public Resource.Unit? District { get; set; }
    public Resource.Unit? Commune { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}

public class RegisterWarehouse {
    public string? Code { get; set; }
    public string Name { get; set; } = null!;
    public Resource.Unit? Province { get; set; }
    public Resource.Unit? District { get; set; }
    public Resource.Unit? Commune { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}

internal class RegisterMerchantHandler(IServiceProvider serviceProvider) : BaseHandler<RegisterMerchantCommand>(serviceProvider) {
    private readonly IAutoGenerationService autoGenerationService = serviceProvider.GetRequiredService<IAutoGenerationService>();

    public override async Task Handle(RegisterMerchantCommand request, CancellationToken cancellationToken) {
        bool isThrow = string.IsNullOrWhiteSpace(request.Merchant?.Code)
            || string.IsNullOrWhiteSpace(request.Merchant?.Name)
            || string.IsNullOrWhiteSpace(request.User?.Username)
            || string.IsNullOrWhiteSpace(request.User?.Password)
            || string.IsNullOrWhiteSpace(request.User?.Name)
            || string.IsNullOrWhiteSpace(request.Store?.Name)
            || string.IsNullOrWhiteSpace(request.Warehouse?.Name);
        ManagedException.ThrowIf(isThrow, Messages.Merchant.Register.Request_Invalid);

        var existed = await db.Merchants.AnyAsync(o => o.Code == request.Merchant!.Code, cancellationToken);
        ManagedException.ThrowIf(existed, Messages.Merchant.Register.Merchant_Existed);

        Merchant merchant = new() {
            Id = NGuidHelper.New(),
            Code = request.Merchant!.Code.Trim().ToLower(),
            Name = request.Merchant.Name.Trim(),
            IsActive = true,
            ExpiredDate = request.Merchant.ExpiredDate <= DateTimeOffset.Now
                ? DateTimeOffset.UtcNow.AddMonths(1)
                : request.Merchant.ExpiredDate,
        };
        await db.Merchants.AddAsync(merchant, cancellationToken);

        User user = new() {
            Id = NGuidHelper.New(),
            MerchantId = merchant.Id,
            Username = request.User!.Username,
            Password = PasswordHasher.Hash(request.User.Password),
            PinCode = PasswordHasher.Hash(request.User.PinCode),
            Name = request.User.Name,
            SearchName = StringHelper.UnsignedUnicode(request.User.Name),
            Phone = request.User.Phone,
            Province = request.User.Province?.Code,
            District = request.User.District?.Code,
            Commune = request.User.Commune?.Code,
            Address = request.User.Address,
            IsActive = true,
            IsAdmin = true,
        };
        await db.Users.AddAsync(user, cancellationToken);

        if (request.Store != null) {
            var model = request.Store;
            if (string.IsNullOrWhiteSpace(model.Code)) {
                model.Code = await autoGenerationService.GenerateCode(
                    merchant.Id,
                    ESetting.AutoGenerateStoreCode,
                    EAutoGenerator.StoreCode,
                    cancellationToken
                );
            }

            Store store = new() {
                Id = NGuidHelper.New(),
                MerchantId = merchant.Id,
                Code = model.Code,
                Name = model.Name,
                SearchName = StringHelper.UnsignedUnicode(model.Name),
                Province = model.Province?.Code,
                District = model.District?.Code,
                Commune = model.Commune?.Code,
                Address = model.Address,
                Phone = model.Phone,
                Email = model.Email,
                IsActive = true,
            };
            await db.Stores.AddAsync(store, cancellationToken);
        }

        if (request.Warehouse != null) {
            var model = request.Warehouse;
            if (string.IsNullOrWhiteSpace(model.Code)) {
                model.Code = await autoGenerationService.GenerateCode(
                    merchant.Id,
                    ESetting.AutoGenerateWarehouseCode,
                    EAutoGenerator.WarehouseCode,
                    cancellationToken
                );
            }

            Warehouse warehouse = new() {
                Id = NGuidHelper.New(),
                MerchantId = merchant.Id,
                Code = model.Code,
                Name = model.Name,
                SearchName = StringHelper.UnsignedUnicode(model.Name),
                Phone = model.Phone,
                Email = model.Email,
                Province = model.Province?.Code,
                District = model.District?.Code,
                Commune = model.Commune?.Code,
                Address = model.Address,
                IsActive = true,
            };
            await db.Warehouses.AddAsync(warehouse, cancellationToken);
        }

        PaymentMethod paymentMethod = new() {
            Id = NGuidHelper.New(),
            MerchantId = merchant.Id,
            Code = await autoGenerationService.GenerateCode(merchant.Id, ESetting.AutoGeneratePaymentMethodCode, EAutoGenerator.PaymentMethodCode, cancellationToken),
            Name = "Tiền mặt",
            IsDefault = true,
        };
        paymentMethod.SearchName = StringHelper.UnsignedUnicode(paymentMethod.Name);
        await db.PaymentMethods.AddAsync(paymentMethod, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);
    }
}
