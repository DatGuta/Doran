using DR.Constant.Enums;
using DR.Database.ExtendModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TuanVu.Database.Models;

namespace DR.Database.Models {
    public partial class GeneralSetting {
        public ESetting Code { get; set; }
        public string DisplayName { get; set; } = null!;
        public string? Description { get; set; }
        public JToken DefaultValue { get; set; } = default!;
        public int OrderIndex { get; set; }

        public virtual ICollection<MerchantSetting>? MerchantSettings { get; set; }
    }

    internal class GeneralSettingConfig : IEntityTypeConfiguration<GeneralSetting> {

        public void Configure(EntityTypeBuilder<GeneralSetting> builder) {
            builder.ToTable(nameof(GeneralSetting));

            builder.HasKey(o => o.Code);

            builder.Property(o => o.Code).HasMaxLength(100);
            builder.Property(o => o.DisplayName).HasMaxLength(255).IsRequired();
            builder.Property(o => o.DefaultValue).HasConversion(o => o.ToString(Formatting.None), o => JToken.Parse(o));

            // fk
            builder.HasMany(o => o.MerchantSettings).WithOne(o => o.GeneralSetting).HasForeignKey(o => o.Code);

            // seed data
            builder.HasData(new GeneralSetting {
                Code = ESetting.AutoGenerateProductCode,
                DisplayName = "Tiền tố mã sản phẩm",
                Description = "Tiền tố mã sản phẩm khi được tạo tự động.",
                OrderIndex = 0,
                DefaultValue = JToken.FromObject(new AutoGenerate {
                    Prefix = "SP",
                    StartNumber = 1,
                    IncrNumber = 1,
                    IsRandomIncrNumber = false,
                    ResetBy = null,
                }),
            },
            new GeneralSetting {
                Code = ESetting.AutoGenerateCategoryCode,
                DisplayName = "Tiền tố mã danh mục",
                Description = "Tiền tố mã danh mục khi được tạo tự động.",
                OrderIndex = 1,
                DefaultValue = JToken.FromObject(new AutoGenerate {
                    Prefix = "DM",
                    StartNumber = 1,
                    IncrNumber = 1,
                    IsRandomIncrNumber = false,
                    ResetBy = null,
                }),
            },
            new GeneralSetting {
                Code = ESetting.AutoGenerateBrandCode,
                DisplayName = "Tiền tố mã thương hiệu",
                Description = "Tiền tố mã thương hiệu khi được tạo tự động.",
                OrderIndex = 2,
                DefaultValue = JToken.FromObject(new AutoGenerate {
                    Prefix = "ThH",
                    StartNumber = 1,
                    IncrNumber = 1,
                    IsRandomIncrNumber = false,
                    ResetBy = null,
                }),
            },
            new GeneralSetting {
                Code = ESetting.AutoGenerateCustomerCode,
                DisplayName = "Tiền tố mã khách hàng",
                Description = "Tiền tố mã khách hàng khi được tạo tự động.",
                OrderIndex = 3,
                DefaultValue = JToken.FromObject(new AutoGenerate {
                    Prefix = "KH",
                    StartNumber = 1,
                    IncrNumber = 1,
                    IsRandomIncrNumber = false,
                    ResetBy = null,
                }),
            },
            new GeneralSetting {
                Code = ESetting.AutoGenerateCustomerGroupCode,
                DisplayName = "Tiền tố mã nhóm khách hàng",
                Description = "Tiền tố mã nhóm khách hàng khi được tạo tự động.",
                OrderIndex = 4,
                DefaultValue = JToken.FromObject(new AutoGenerate {
                    Prefix = "NKH",
                    StartNumber = 1,
                    IncrNumber = 1,
                    IsRandomIncrNumber = false,
                    ResetBy = null,
                }),
            },
            new GeneralSetting {
                Code = ESetting.AutoGenerateOrderNo,
                DisplayName = "Tiền tố mã đơn hàng",
                Description = "Tiền tố mã đơn hàng khi được tạo tự động.",
                OrderIndex = 5,
                DefaultValue = JToken.FromObject(new AutoGenerate {
                    Prefix = "ĐH",
                    StartNumber = 1,
                    IncrNumber = 1,
                    IsRandomIncrNumber = false,
                    ResetBy = EDateTimePeriod.Day,
                }),
            },
            new GeneralSetting {
                Code = ESetting.AutoGenerateSupplierCode,
                DisplayName = "Tiền tố mã nhà cung cấp",
                Description = "Tiền tố mã nhà cung cấp khi được tạo tự động.",
                OrderIndex = 6,
                DefaultValue = JToken.FromObject(new AutoGenerate {
                    Prefix = "NCC",
                    StartNumber = 1,
                    IncrNumber = 1,
                    IsRandomIncrNumber = false,
                    ResetBy = null,
                }),
            },
            new GeneralSetting {
                Code = ESetting.AutoGenerateWarehouseCode,
                DisplayName = "Tiền tố mã kho",
                Description = "Tiền tố mã kho khi được tạo tự động.",
                OrderIndex = 7,
                DefaultValue = JToken.FromObject(new AutoGenerate {
                    Prefix = "K",
                    StartNumber = 1,
                    IncrNumber = 1,
                    IsRandomIncrNumber = false,
                    ResetBy = null,
                }),
            },
            new GeneralSetting {
                Code = ESetting.AutoGenerateWarehouseImportCode,
                DisplayName = "Tiền tố mã phiếu nhập kho",
                Description = "Tiền tố mã phiếu nhập kho khi được tạo tự động.",
                OrderIndex = 8,
                DefaultValue = JToken.FromObject(new AutoGenerate {
                    Prefix = "NK",
                    StartNumber = 1,
                    IncrNumber = 1,
                    IsRandomIncrNumber = false,
                    ResetBy = EDateTimePeriod.Month,
                }),
            },
            new GeneralSetting {
                Code = ESetting.AutoGenerateWarehouseExportCode,
                DisplayName = "Tiền tố mã phiếu xuất kho",
                Description = "Tiền tố mã phiếu xuất kho khi được tạo tự động.",
                OrderIndex = 9,
                DefaultValue = JToken.FromObject(new AutoGenerate {
                    Prefix = "XK",
                    StartNumber = 1,
                    IncrNumber = 1,
                    IsRandomIncrNumber = false,
                    ResetBy = EDateTimePeriod.Day,
                }),
            },
            new GeneralSetting {
                Code = ESetting.AutoGenerateWarehouseTransferCode,
                DisplayName = "Tiền tố mã phiếu chuyển kho",
                Description = "Tiền tố mã phiếu chuyển kho khi được tạo tự động.",
                OrderIndex = 10,
                DefaultValue = JToken.FromObject(new AutoGenerate {
                    Prefix = "CK",
                    StartNumber = 1,
                    IncrNumber = 1,
                    IsRandomIncrNumber = false,
                    ResetBy = EDateTimePeriod.Month,
                }),
            },
            new GeneralSetting {
                Code = ESetting.AutoGenerateWarehouseRefundCode,
                DisplayName = "Tiền tố mã phiếu trả hàng",
                Description = "Tiền tố mã phiếu trả hàng khi được tạo tự động.",
                OrderIndex = 11,
                DefaultValue = JToken.FromObject(new AutoGenerate {
                    Prefix = "TH",
                    StartNumber = 1,
                    IncrNumber = 1,
                    IsRandomIncrNumber = false,
                    ResetBy = EDateTimePeriod.Month,
                }),
            },
            new GeneralSetting {
                Code = ESetting.AutoGenerateWarehouseAdjustmentCode,
                DisplayName = "Tiền tố mã phiếu kiểm kho",
                Description = "Tiền tố mã phiếu kiểm kho khi được tạo tự động.",
                OrderIndex = 12,
                DefaultValue = JToken.FromObject(new AutoGenerate {
                    Prefix = "KK",
                    StartNumber = 1,
                    IncrNumber = 1,
                    IsRandomIncrNumber = false,
                    ResetBy = EDateTimePeriod.Year,
                }),
            },
            new GeneralSetting {
                Code = ESetting.AutoGenerateStoreCode,
                DisplayName = "Tiền tố mã cửa hàng",
                Description = "Tiền tố mã cửa hàng khi được tạo tự động",
                OrderIndex = 13,
                DefaultValue = JToken.FromObject(new AutoGenerate {
                    Prefix = "CH",
                    StartNumber = 1,
                    IncrNumber = 1,
                    IsRandomIncrNumber = false,
                    ResetBy = null,
                }),
            },
            new GeneralSetting {
                Code = ESetting.AutoGenerateRoleCode,
                DisplayName = "Tiền tố mã phần quyền",
                Description = "Tiền tố mã phần quyền khi được tạo tự động.",
                OrderIndex = 14,
                DefaultValue = JToken.FromObject(new AutoGenerate {
                    Prefix = "PQ",
                    StartNumber = 1,
                    IncrNumber = 1,
                    IsRandomIncrNumber = false,
                    ResetBy = null,
                }),
            },
            new GeneralSetting {
                Code = ESetting.AutoGeneratePaymentMethodCode,
                DisplayName = "Tiền tố mã phương thức thanh toán",
                Description = "Tiền tố mã phương thức thanh toán khi được tạo tự động.",
                OrderIndex = 15,
                DefaultValue = JToken.FromObject(new AutoGenerate {
                    Prefix = "PTTT",
                    StartNumber = 1,
                    IncrNumber = 1,
                    IsRandomIncrNumber = false,
                    ResetBy = null,
                }),
            },
            new GeneralSetting {
                Code = ESetting.EmailSettingCode,
                DisplayName = "Báo cáo qua email",
                Description = "Báo cáo doanh thu ngày/tuần/tháng qua email.",
                OrderIndex = 16,
                DefaultValue = JToken.FromObject(new EmailReport {
                    Hour = 0,
                    Day = false,
                    Week = false,
                    Month = false,
                    IsSend = false,
                    Emails = new(),
                }),
            },
            new GeneralSetting {
                Code = ESetting.AutoGenerateReceiptCode,
                DisplayName = "Tiền tố mã phiếu thu",
                Description = "Tiền tố mã phiếu thu khi được tạo tự động.",
                OrderIndex = 17,
                DefaultValue = JToken.FromObject(new AutoGenerate {
                    Prefix = "PT",
                    StartNumber = 1,
                    IncrNumber = 1,
                    IsRandomIncrNumber = false,
                    ResetBy = EDateTimePeriod.Day,
                }),
            },
            new GeneralSetting {
                Code = ESetting.AutoGeneratePaymentCode,
                DisplayName = "Tiền tố mã phiếu chi",
                Description = "Tiền tố mã phiếu chi khi được tạo tự động.",
                OrderIndex = 18,
                DefaultValue = JToken.FromObject(new AutoGenerate {
                    Prefix = "PC",
                    StartNumber = 1,
                    IncrNumber = 1,
                    IsRandomIncrNumber = false,
                    ResetBy = EDateTimePeriod.Day,
                }),
            },
            new GeneralSetting {
                Code = ESetting.AutoExportOrder,
                DisplayName = "Tự động xuất khi tạo đơn hàng",
                Description = "Tự động cập nhật số lượng xuất khi tạo hoặc sửa đơn hàng.",
                OrderIndex = 19,
                DefaultValue = JToken.FromObject(false),
            },
            new GeneralSetting {
                Code = ESetting.NumberFormat,
                DisplayName = "Định dạng số",
                Description = "Định dạng phần nghìn và phần thập phân của số.",
                OrderIndex = 20,
                DefaultValue = JToken.FromObject(new NumberFormat()),
            });
        }
    }
}
