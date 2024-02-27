

using DR.Constant.Enums;
using DR.Database;

namespace DR.Operators.AutoGenerate {

    public interface IAutoGenerateOperator {
        ESetting Handler { get; }

        Task<string?> GetLastCode(DrDbContext db, Guid merchantId, string prefix, List<string> excludes);
    }
}
