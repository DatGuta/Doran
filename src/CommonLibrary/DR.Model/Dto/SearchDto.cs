namespace DR.Models.Dto {

    public enum ESearchType {
        Product = 1,
        Brand = 2,
    }

    public class TempSearchDto : SearchDto {
        public string SearchName { get; set; } = string.Empty;
    }

    public class SearchDto {
        public ESearchType Type { get; set; }
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public ImageDto? Image { get; set; }

        public SearchDto() {
        }

        public SearchDto(SearchDto other) {
            Type = other.Type;
            Id = other.Id;
            Name = other.Name;
            Image = other.Image;
        }
    }
}
