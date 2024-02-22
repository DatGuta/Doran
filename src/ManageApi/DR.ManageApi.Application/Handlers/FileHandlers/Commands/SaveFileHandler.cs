using DR.Common.Exceptions;
using DR.Helper;
using DR.Message;

namespace DR.ManageApi.Application.Handlers.FileHandlers.Commands;

public class SaveFileCommand : ModelRequest<FileDto> { }

internal class SaveFileHandler(IServiceProvider serviceProvider) : BaseHandler<SaveFileCommand>(serviceProvider) {

    public override async Task Handle(SaveFileCommand request, CancellationToken cancellationToken) {
        var merchantId = request.MerchantId;
        var userId = request.UserId;
        var model = request.Model;
        if (model.Data == null || model.Data.Length == 0)
            return;

        if (string.IsNullOrWhiteSpace(model.Id)) {
            model.Id = NGuidHelper.New();
            await Create(merchantId, userId, model);
            return;
        }
        await Update(merchantId, userId, model);
    }

    private async Task Create(string merchantId, string userId, FileDto model) {
        var entity = new Database.Models.File() {
            Id = model.Id ?? NGuidHelper.New(),
            MerchantId = merchantId,
            Type = model.Type,
            ItemId = model.ItemId,
            ItemType = model.ItemType,
            Name = model.Name,
            UploadBy = userId,
            UploadDate = DateTimeOffset.Now,
        };
        entity.Path = await UploadFile(entity, model.Data);
        await db.AddAsync(entity);
        await db.SaveChangesAsync();
    }

    private async Task Update(string merchantId, string userId, FileDto model) {
        var entity = await db.Files.FirstOrDefaultAsync(o => o.MerchantId == merchantId && o.Id == model.Id);
        ManagedException.ThrowIf(entity == null, Messages.File.File_Error);

        entity.Name = model.Name;
        entity.UploadBy = userId;
        entity.UploadDate = DateTimeOffset.Now;

        await FtpHelper.DeleteFile(entity.Path, configuration);
        entity.Path = await UploadFile(entity, model.Data);

        await db.SaveChangesAsync();
    }

    private async Task<string> UploadFile(Database.Models.File item, byte[]? data) {
        var (directories, filename) = GetFilePath(item);
        await FtpHelper.UploadFile(directories, filename, data, configuration);
        return filename;
    }

    private static (string[], string) GetFilePath(Database.Models.File file) {
        string fileType = Enum.IsDefined(file.Type) ? file.Type.ToString().ToLower() : "temporary";
        string itemType = Enum.IsDefined(file.ItemType) ? file.ItemType.ToString().ToLower() : "other";
        string extentions = Path.GetExtension(file.Name);

        string[] directories = new string[] {
                $"{fileType}/{file.MerchantId}",
                $"{fileType}/{file.MerchantId}/{itemType}"
            };

        return (directories, $"{fileType}/{file.MerchantId}/{itemType}/{file.Id}{extentions}");
    }
}
