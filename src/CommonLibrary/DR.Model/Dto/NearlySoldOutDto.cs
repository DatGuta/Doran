namespace DR.Models.Dto {
    public class NearlySoldOutDto {

        /// <summary>
        /// Product code
        /// </summary>
        public string ProductCode { get; set; } = string.Empty;

        /// <summary>
        /// Product name
        /// </summary>
        public string Product { get; set; } = string.Empty;

        /// <summary>
        /// Warehouse name
        /// </summary>
        public string Warehouse { get; set; } = string.Empty;

        /// <summary>
        /// On hand of product in warehouse
        /// </summary>
        public decimal Value { get; set; }
    }
}
