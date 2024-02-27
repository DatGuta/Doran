using DR.Constant.Enums;

namespace DR.Common.Models {

    public class OperatorFillter {
        public string FieldName { get; set; } = string.Empty;
        public ComparisonType ComparisonType { get; set; }
        public object Value { get; set; } = null!;
    }
}
