using System.Collections.Generic;
using System.ComponentModel;

namespace DR.Export.CustomerExport {
    public partial class CustomerExportData {

        [Description("STT")]
        public int No { get; set; }

        [Description("Mã khách hàng")]
        public string Code { get; set; }

        [Description("Tên khách hàng")]
        public string Name { get; set; }

        [Description("Số điện thoại")]
        public string Phone { get; set; }

        [Description("Địa chỉ")]
        public string Address { get; set; }

        [Description("Thành phố/Tỉnh")]
        public string Province { get; set; }

        [Description("Quận/Huyện")]
        public string District { get; set; }

        [Description("Phường/Xã")]
        public string Commune { get; set; }
    }
}
