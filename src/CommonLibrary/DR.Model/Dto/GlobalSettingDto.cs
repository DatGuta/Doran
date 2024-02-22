using DR.Database.ExtendModels;

namespace DR.Models.Dto {
    public class GlobalSettingDto {
        public NumberFormat NumberFormat { get; set; } = new NumberFormat();

        public static GlobalSettingDto Default() {
            return new GlobalSettingDto();
        }
    }
}
