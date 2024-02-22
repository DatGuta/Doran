using DR.Constant.Enums;

namespace DR.ManageApi.Application.Handlers.FileHandlers.Queries;

public class ListFileQuery : Request<ListFileData> {
    public List<EItemType> Codes { get; set; } = [];
}

public class ListFileData : PaginatedList<FileDto> { }

internal class ListFileHandler(IServiceProvider serviceProvider) : BaseHandler<ListFileQuery, ListFileData>(serviceProvider) {

    public override async Task<ListFileData> Handle(ListFileQuery request, CancellationToken cancellationToken) {
        if (request.Codes.Count == 0) return new();

        var files = await db.Files.AsNoTracking()
            .Where(o => o.MerchantId == request.MerchantId && o.Type == EFile.Templates && request.Codes.Contains(o.ItemType))
            .ToListAsync(cancellationToken);

        foreach (var itemType in request.Codes) {
            var file = files.FirstOrDefault(o => o.ItemType == itemType);
            if (file == null) files.Add(new Database.Models.File() {
                ItemType = itemType,
                Type = EFile.Templates,
            });
        }

        return new() {
            Items = files.Select(o => FileDto.FromEntity(o, url)).ToList(),
            Count = files.Count
        };
    }
}
