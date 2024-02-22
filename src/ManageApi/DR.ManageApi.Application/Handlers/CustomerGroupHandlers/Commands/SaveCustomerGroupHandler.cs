using DR.Common.Exceptions;
using DR.Common.Extensions;
using DR.Common.Lock;
using DR.Constant.Enums;
using DR.Contexts.AuditContexts;
using DR.ManageApi.Domain.Actions.AutoGenerate;
using DR.ManageApi.Domain.Services.Interfaces;
using DR.Message;

namespace DR.ManageApi.Application.Handlers.CustomerGroupHandlers.Commands;

public class SaveCustomerGroupCommand : ModelRequest<CustomerGroupDto, string> { }

internal class SaveCustomerGroupHandler(IServiceProvider serviceProvider) : BaseHandler<SaveCustomerGroupCommand, string>(serviceProvider) {
    private readonly IAutoGenerationService autoGenerationService = serviceProvider.GetRequiredService<IAutoGenerationService>();
    private readonly IMediator mediator = serviceProvider.GetRequiredService<IMediator>();

    public override async Task<string> Handle(SaveCustomerGroupCommand request, CancellationToken cancellationToken) {
        var merchantId = request.MerchantId;
        var userId = request.UserId;
        var model = request.Model;

        model.Code = model.Code.Trim().ToUpper();
        model.Name = model.Name.Trim();

        return string.IsNullOrWhiteSpace(model.Id)
            ? await Locker.LockByKey($"Merchant:{request.MerchantId}:CustomerGroup",
                () => Create(merchantId, userId, model, cancellationToken))
            : await Update(merchantId, userId, model, cancellationToken);
    }

    private async Task<string> Create(string merchantId, string userId, CustomerGroupDto model, CancellationToken cancellationToken) {
        if (string.IsNullOrEmpty(model.Code)) {
            model.Code = await autoGenerationService.GenerateCode(merchantId, ESetting.AutoGenerateCustomerGroupCode, EAutoGenerator.CustomerGroupCode, cancellationToken);
        } else {
            var existed = await db.CustomerGroups.AnyAsync(o => o.MerchantId == merchantId && o.Code == model.Code, cancellationToken);
            ManagedException.ThrowIf(existed, Messages.Customer.CreateOrUpdate.Customer_Existed);
        }

        var entity = model.ToEntity(merchantId);
        await db.CustomerGroups.AddAsync(entity, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        await mediator.Publish(new CusGrpCAuditContext(merchantId, userId, entity), cancellationToken);
        return entity.Id;
    }

    private async Task<string> Update(string merchantId, string userId, CustomerGroupDto model, CancellationToken cancellationToken) {
        var entity = await db.CustomerGroups.FirstOrDefaultAsync(o => o.MerchantId == merchantId && o.Id == model.Id, cancellationToken);
        ManagedException.ThrowIfNull(entity, Messages.CustomerGroup.CreateOrUpdate.Customer_NotFound);

        var origin = entity.Clone();

        entity.Name = model.Name;
        entity.Description = model.Description;
        entity.ModifiedDate = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(cancellationToken);
        await mediator.Publish(new CusGrpUAuditContext(merchantId, userId, origin, entity), cancellationToken);

        return entity.Id;
    }
}
