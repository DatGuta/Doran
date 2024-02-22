//using System.Globalization;
//using System.Text;
//using CsvHelper;
//using ExcelDataReader;
//using FMS.Common.Exceptions;
//using FMS.Constant.Enums;
//using FMS.Helper;
//using FMS.Message;

//namespace FMS.ManageApi.Application.Handlers.CustomerHandlers.Commands;

//public class ImportCustomerCommand : Request<ImportCustomerData> {
//    public byte[]? File { get; set; }
//    public EImportType Type { get; set; }
//}

//public class ImportCustomerData : PaginatedList<ImportCustomerDto> { }

//internal class ImportCustomerHandler(IServiceProvider serviceProvider) : BaseHandler<ImportCustomerCommand, ImportCustomerData>(serviceProvider) {
//    private readonly IMediator mediator = serviceProvider.GetRequiredService<IMediator>();

//    public override async Task<ImportCustomerData> Handle(ImportCustomerCommand request, CancellationToken cancellationToken) {
//        var data = new List<ImportCustomerDto>();
//        try {
//            if (request.Type == EImportType.CSV) {
//                data = ImportCSV(request.File!);
//            } else if (request.Type == EImportType.XLS || request.Type == EImportType.XLSX) {
//                data = ImportExcel(request.File!, request.Type);
//            }
//        } catch {
//            ManagedException.Throw(Messages.Customer.Import.Customer_NotRead);
//        }

//        return await mediator.Send(new CheckImportCustomerCommand { Data = data, MerchantId = request.MerchantId, UserId = request.UserId }, cancellationToken);
//    }

//    private static List<ImportCustomerDto> ImportCSV(byte[] file) {
//        List<ImportCustomerDto> result;
//        UTF8Encoding enc = new();
//        using (var reader = new StringReader(enc.GetString(file)))
//        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture)) {
//            csv.Context.RegisterClassMap<ImportCustomerMap>();
//            result = csv.GetRecords<ImportCustomerDto>().Where(o => !o.IsEmpty()).ToList();
//        }
//        return result;
//    }

//    private static List<ImportCustomerDto> ImportExcel(byte[] file, EImportType type) {
//        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
//        var config = new ExcelReaderConfiguration() {
//            FallbackEncoding = new UTF8Encoding(true),
//            LeaveOpen = true,
//        };

//        var data = new List<ImportCustomerDto>();

//        using (var stream = new MemoryStream(file))
//        using (var reader = type == EImportType.XLS
//            ? ExcelReaderFactory.CreateReader(stream, config)
//            : ExcelReaderFactory.CreateOpenXmlReader(stream, config)) {
//            while (reader.Read()) {
//                if (reader.Depth == 0) continue;

//                var item = new ImportCustomerDto() {
//                    Id = NGuidHelper.New(),
//                    Line = reader.Depth + 1,
//                    Code = reader[0]?.ToString(),
//                    Name = reader[1]?.ToString(),
//                    Phone = reader[2]?.ToString(),
//                    Address = reader[3]?.ToString(),
//                    Commune = new() { Name = reader[4]?.ToString() ?? "" },
//                    District = new() { Name = reader[5]?.ToString() ?? "" },
//                    Province = new() { Name = reader[6]?.ToString() ?? "" },
//                };
//                if (!item.IsEmpty()) data.Add(item);
//            }
//        }
//        return data;
//    }
//}
