using System.ComponentModel;
using CsvHelper.Configuration;
using DR.Message;
using Newtonsoft.Json;

namespace DR.Models.Dto {

    public class ImportCustomerDto {
        public string? Id { get; set; }
        public int Line { get; set; }

        [Description("Mã khách hàng")]
        public string? Code { get; set; }

        [Description("Tên khách hàng")]
        public string? Name { get; set; }

        [Description("Số điện thoại")]
        public string? Phone { get; set; }

        [Description("Địa chỉ")]
        public string? Address { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Description("Phường xã")]
        public Unit? Commune { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Description("Quận/Huyện")]
        public Unit? District { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Description("Thành phố/Tỉnh")]
        public Unit? Province { get; set; }

        public List<ImportStatus> ImportStatuses { get; set; } = new List<ImportStatus>();

        public bool IsEmpty() {
            if (string.IsNullOrWhiteSpace(Name) && string.IsNullOrWhiteSpace(Code)
                           && string.IsNullOrWhiteSpace(Phone) && string.IsNullOrWhiteSpace(Address)
                           && string.IsNullOrWhiteSpace(Province?.Name) && string.IsNullOrWhiteSpace(District?.Name)
                           && string.IsNullOrWhiteSpace(Commune?.Name)) return true;
            return false;
        }

        public void CheckImport(List<string> codes, List<AddressMap> addr) {
            ClearStatus();
            CheckName();
            CheckCode(codes);
            CheckProvince(addr);
            CheckDistrict(addr);
            CheckCommune(addr);
        }

        private void ClearStatus() {
            ImportStatuses = new();
        }

        private void CheckName() {
            if (string.IsNullOrWhiteSpace(Name) && !ImportStatuses.Exists(o => o.Key == nameof(Name))) {
                ImportStatuses.Add(new ImportStatus {
                    Key = nameof(Name),
                    Message = Messages.Customer.Import.Customer_NameNotNull,
                    Status = EImportStatus.Error,
                });
            }
        }

        private void CheckCode(List<string> codes) {
            string? message = null;
            if (!string.IsNullOrWhiteSpace(Code) && codes.Count(o => o == Code.Trim().ToUpper()) > 1) {
                message = Messages.Customer.Import.Customer_IsExit;
            }

            if (!string.IsNullOrWhiteSpace(message)) {
                ImportStatuses.Add(new ImportStatus() {
                    Key = nameof(Code),
                    Message = message,
                    Status = EImportStatus.Error,
                });
            }
        }

        private void CheckProvince(List<AddressMap> addr) {
            var provinceName = StringHelper.ReplaceSpace(Province?.Name?.Replace("-", " "), true) ?? "";
            Province = addr?.Find(o => o.Name.Contains(provinceName))?.Unit;

            if (Province == null) {
                ImportStatuses.Add(new ImportStatus() {
                    Key = nameof(Province),
                    Status = EImportStatus.Warning,
                    Message = Messages.Customer.Import.Customer_NotRead
                });
            }
        }

        private void CheckDistrict(List<AddressMap> addr) {
            if (Province == null) {
                if (!string.IsNullOrWhiteSpace(District?.Name)) {
                    ImportStatuses.Add(new ImportStatus() {
                        Key = nameof(District),
                        Status = EImportStatus.Warning,
                        Message = Messages.Customer.Import.Customer_NotRead
                    });
                }
                District = null;
                return;
            }

            var districtName = StringHelper.UnsignedUnicode(District?.Name, "");
            District = addr?.Find(o => !string.IsNullOrWhiteSpace(districtName) && o.Name.Contains(districtName))?.Unit;

            if (District == null && !string.IsNullOrWhiteSpace(districtName)) {
                ImportStatuses.Add(new ImportStatus() {
                    Key = nameof(District),
                    Status = EImportStatus.Warning,
                    Message = Messages.Customer.Import.Customer_NotRead
                });
            }
        }

        private void CheckCommune(List<AddressMap> addr) {
            if (District == null) {
                if (!string.IsNullOrWhiteSpace(Commune?.Name)) {
                    ImportStatuses.Add(new ImportStatus() {
                        Key = nameof(Commune),
                        Status = EImportStatus.Warning,
                        Message = Messages.Customer.Import.Customer_NotRead
                    });
                }
                Commune = null;
                return;
            }

            var communeName = StringHelper.UnsignedUnicode(Commune?.Name, "");
            Commune = addr?.Find(o => !string.IsNullOrWhiteSpace(communeName) && o.Name.Contains(communeName))?.Unit;

            if (Commune == null && !string.IsNullOrWhiteSpace(communeName)) {
                ImportStatuses.Add(new ImportStatus() {
                    Key = nameof(Commune),
                    Status = EImportStatus.Warning,
                    Message = Messages.Customer.Import.Customer_NotRead
                });
            }
        }
    }

    public class ImportStatus {
        public string Key { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public EImportStatus Status { get; set; }
    }

    public class ImportCustomerMap : ClassMap<ImportCustomerDto> {

        public ImportCustomerMap() {
            Map(o => o.Code).Index(0);
            Map(o => o.Name).Index(1);
            Map(o => o.Phone).Index(2);
            Map(o => o.Address).Index(3);
            Map(o => o.Commune!.Name).Index(4);
            Map(o => o.District!.Name).Index(5);
            Map(o => o.Province!.Name).Index(6);
            Map(o => o.Line).Convert(row => row.Row.Context.Parser.RawRow);
        }
    }
}
