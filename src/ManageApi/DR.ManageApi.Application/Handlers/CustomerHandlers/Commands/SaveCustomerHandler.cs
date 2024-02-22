using DR.Common.Exceptions;
using DR.Common.Extensions;
using DR.Common.Hashers;
using DR.Common.Lock;
using DR.Constant.Enums;
using DR.Contexts.AuditContexts;
using DR.Contexts.NormalContexts;
using DR.Database.Models;
using DR.Helper;
using DR.ManageApi.Domain.Actions.AutoGenerate;
using DR.ManageApi.Domain.Services.Interfaces;
using DR.Message;
using MassTransit;

namespace FMS.ManageApi.Application.Handlers.CustomerHandlers.Commands;

public class SaveCustomerCommand : ModelRequest<CustomerDto, string> { }

internal class SaveCustomerHandler(IServiceProvider serviceProvider) : BaseHandler<SaveCustomerCommand, string>(serviceProvider) {
    private readonly IBus bus = serviceProvider.GetRequiredService<IBus>();
    private readonly IMediator mediator = serviceProvider.GetRequiredService<IMediator>();
    private readonly IAutoGenerationService autoGenerationService = serviceProvider.GetRequiredService<IAutoGenerationService>();

    public override async Task<string> Handle(SaveCustomerCommand request, CancellationToken cancellationToken) {
        var merchantId = request.MerchantId;
        var userId = request.UserId;
        var model = request.Model;
        model.Code = model.Code?.Trim().ToUpper();
        model.Name = model.Name.Trim();
        model.Phone = model.Phone?.Trim();

        return string.IsNullOrWhiteSpace(model.Id)
            ? await Locker.LockByKey($"Merchant:{request.MerchantId}:Customer",
                () => Create(merchantId, userId, model, cancellationToken))
            : await Update(merchantId, userId, model, cancellationToken);
    }

    private async Task<string> Create(string merchantId, string userId, CustomerDto model, CancellationToken cancellationToken) {
        if (string.IsNullOrEmpty(model.Code)) {
            model.Code = await autoGenerationService.GenerateCode(merchantId, ESetting.AutoGenerateCustomerCode, EAutoGenerator.CustomerCode, cancellationToken);
        } else {
            var existed = await db.Customers.AnyAsync(o => o.MerchantId == merchantId && o.Code == model.Code, cancellationToken);
            ManagedException.ThrowIf(existed, Messages.Customer.CreateOrUpdate.Customer_Existed);
        }

        var entity = model.ToEntity(merchantId);
        entity.CustomerPhones = GetCustomerPhones(entity.Id, model.Phone);

        await db.Customers.AddAsync(entity, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        await mediator.Publish(new CustomerCAuditContext(merchantId, userId, entity.Clone()), cancellationToken);
        if (!string.IsNullOrWhiteSpace(entity.CustomerGroupId)) {
            await bus.Publish(new UpdateNoOfCusByCusGrpContext(merchantId, new HashSet<string> { entity.CustomerGroupId }), cancellationToken);
        }

        return entity.Id;
    }

    private async Task<string> Update(string merchantId, string userId, CustomerDto model, CancellationToken cancellationToken) {
        var existed = await db.Customers.AnyAsync(o => o.MerchantId == merchantId && o.Id != model.Id && o.Code == model.Code, cancellationToken);
        ManagedException.ThrowIf(existed, Messages.Customer.CreateOrUpdate.Customer_Existed);

        var entity = await db.Customers.Include(o => o.CustomerPhones)
            .FirstOrDefaultAsync(o => o.MerchantId == merchantId && o.Id == model.Id, cancellationToken);
        ManagedException.ThrowIf(entity == null, Messages.Customer.CreateOrUpdate.Customer_NotFound);

        var originalEntity = entity.Clone();

        entity.UpdateFrom(model.ToEntity(merchantId));
        entity.CustomerPhones = GetCustomerPhones(entity.Id, model.Phone, entity.CustomerPhones);

        await db.SaveChangesAsync(cancellationToken);

        await mediator.Publish(new CustomerUAuditContext(merchantId, userId, originalEntity, entity.Clone()), cancellationToken);

        if (originalEntity.CustomerGroupId != entity.CustomerGroupId) {
            var set = new HashSet<string>();
            if (!string.IsNullOrWhiteSpace(originalEntity.CustomerGroupId))
                set.Add(originalEntity.CustomerGroupId);
            if (!string.IsNullOrWhiteSpace(entity.CustomerGroupId))
                set.Add(entity.CustomerGroupId);

            if (set.Count > 0) {
                await bus.Publish(new UpdateNoOfCusByCusGrpContext(merchantId, set), cancellationToken);
            }
        }
        return entity.Id;
    }

    private static List<CustomerPhone> GetCustomerPhones(string customerId, string? phone, ICollection<CustomerPhone>? dbCustomerPhones = null) {
        return phone?.Split(",;-/|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
            .Select(o => o.Replace(" ", "").Replace(".", ""))
            .Where(o => !string.IsNullOrWhiteSpace(o))
            .Select(o => {
                string? searchPhone = PhoneHasher.Encrypt(o);
                string? searchLastPhone = PhoneHasher.Encrypt(StringHelper.GetLast(o));

                string id = NGuidHelper.New(dbCustomerPhones?.FirstOrDefault(o => o.SearchPhone == searchPhone && o.SearchLastPhone == searchLastPhone)?.Id);

                return new CustomerPhone {
                    Id = id,
                    CustomerId = customerId,
                    SearchPhone = searchPhone,
                    SearchLastPhone = searchLastPhone,
                };
            }).ToList() ?? [];
    }
}
