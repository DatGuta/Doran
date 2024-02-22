using DR.Constant.Enums;

namespace DR.Database.Models;

public partial class Permission : ICloneable {
    public string Id { get; set; } = null!;
    public string? ParentId { get; set; }
    public string ClaimName { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
    public bool IsClaim { get; set; }
    public int OrderIndex { get; set; }
    public EPermission Type { get; set; }

    public virtual ICollection<RolePermission>? RolePermissions { get; set; }

    public object Clone() {
        return new Permission {
            Id = Id,
            ParentId = ParentId,
            ClaimName = ClaimName,
            DisplayName = DisplayName,
            IsDefault = IsDefault,
            IsActive = IsActive,
            IsClaim = IsClaim,
            OrderIndex = OrderIndex,
            Type = Type,
        };
    }
}

internal class PermissionConfig : IEntityTypeConfiguration<Permission> {

    public void Configure(EntityTypeBuilder<Permission> builder) {
        builder.ToTable(nameof(Permission));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);
        builder.Property(o => o.ParentId).HasMaxLength(32);

        builder.Property(o => o.ClaimName).HasMaxLength(50).IsRequired();
        builder.Property(o => o.DisplayName).HasMaxLength(50).IsRequired();
        builder.Property(o => o.Type).HasMaxLength(20).IsRequired();

        // fk
        builder.HasMany(o => o.RolePermissions).WithOne(o => o.Permission).HasForeignKey(o => o.PermissionId);

        // seed data
        var index = 0;
        builder.HasData(new Permission {
            Id = "ec0f270b424249438540a16e9157c0c8",
            ClaimName = "BO",
            DisplayName = "Trang quản lý",
            IsDefault = true,
            IsActive = true,
            IsClaim = false,
            OrderIndex = index++,
            Type = EPermission.Web,
        });

        builder.HasData(new Permission {
            Id = "b47bbb68c29e4880bb3a230620ce4e6e",
            ParentId = "ec0f270b424249438540a16e9157c0c8",
            ClaimName = "BO.Dashboard",
            DisplayName = "Tổng quan",
            IsDefault = true,
            IsActive = true,
            IsClaim = true,
            OrderIndex = index++,
            Type = EPermission.Web,
        });

        builder.HasData(new Permission {
            Id = "59287440677a471b87e9b5c336a4e48f",
            ParentId = "ec0f270b424249438540a16e9157c0c8",
            ClaimName = "BO.Summary",
            DisplayName = "Tổng hợp",
            IsDefault = false,
            IsActive = true,
            IsClaim = true,
            OrderIndex = index++,
            Type = EPermission.Web,
        },
        new Permission {
            Id = "ffeafd7c8e034269a8d2123290bf6281",
            ParentId = "59287440677a471b87e9b5c336a4e48f",
            ClaimName = "BO.Summary.Sale",
            DisplayName = "Tổng hợp bán hàng",
            IsDefault = false,
            IsActive = true,
            IsClaim = true,
            OrderIndex = index++,
            Type = EPermission.Web,
        });

        builder.HasData(new Permission {
            Id = "a8845d8773f345d9b572ef4ee04136cf",
            ParentId = "ec0f270b424249438540a16e9157c0c8",
            ClaimName = "BO.Product",
            DisplayName = "Quản lý sản phẩm",
            IsDefault = true,
            IsActive = true,
            IsClaim = true,
            OrderIndex = index++,
            Type = EPermission.Web,
        });

        builder.HasData(new Permission {
            Id = "55b02d024daf4de99a0c4fc54eaccbff",
            ParentId = "ec0f270b424249438540a16e9157c0c8",
            ClaimName = "BO.Customer",
            DisplayName = "Quản lý khách hàng",
            IsDefault = true,
            IsActive = true,
            IsClaim = true,
            OrderIndex = index++,
            Type = EPermission.Web,
        });

        builder.HasData(new Permission {
            Id = "66c6f7c0c0c142348d05d4b04e4b1306",
            ParentId = "ec0f270b424249438540a16e9157c0c8",
            ClaimName = "BO.Order",
            DisplayName = "Quản lý đơn hàng",
            IsDefault = true,
            IsActive = true,
            IsClaim = true,
            OrderIndex = index++,
            Type = EPermission.Web,
        },
        new Permission {
            Id = "4a180f5ed9cc441d9c9a9364f32f8f56",
            ParentId = "66c6f7c0c0c142348d05d4b04e4b1306",
            ClaimName = "BO.Order.Update",
            DisplayName = "Cập nhật đơn hàng",
            IsDefault = false,
            IsActive = true,
            IsClaim = true,
            OrderIndex = index++,
            Type = EPermission.Web,
        });

        builder.HasData(new Permission {
            Id = "f65313e6203e47888e5a08603a7e9941",
            ParentId = "ec0f270b424249438540a16e9157c0c8",
            ClaimName = "BO.ReceiptPayment",
            DisplayName = "Quản lý công nợ",
            IsDefault = false,
            IsActive = true,
            IsClaim = true,
            OrderIndex = index++,
            Type = EPermission.Web,
        });

        builder.HasData(new Permission {
            Id = "a9f2dbe25fbb4831b30fd01781f9dbdd",
            ParentId = "ec0f270b424249438540a16e9157c0c8",
            ClaimName = "BO.Supplier",
            DisplayName = "Quản lý nhà cung cấp",
            IsDefault = false,
            IsActive = true,
            IsClaim = true,
            OrderIndex = index++,
            Type = EPermission.Web,
        });

        builder.HasData(new Permission {
            Id = "568867b3a6e340d2990a47ad26520921",
            ParentId = "ec0f270b424249438540a16e9157c0c8",
            ClaimName = "BO.Warehouse",
            DisplayName = "Quản lý kho",
            IsDefault = false,
            IsActive = true,
            IsClaim = true,
            OrderIndex = index++,
            Type = EPermission.Web,
        });

        builder.HasData(new Permission {
            Id = "ea4e3f343e714710ac7b78e3c7e36fe8",
            ParentId = "ec0f270b424249438540a16e9157c0c8",
            ClaimName = "BO.Store",
            DisplayName = "Quản lý cửa hàng",
            IsDefault = false,
            IsActive = true,
            IsClaim = true,
            OrderIndex = index++,
            Type = EPermission.Web,
        });

        builder.HasData(new Permission {
            Id = "3455b6af67b94183b8a69fd08a08c52b",
            ParentId = "ec0f270b424249438540a16e9157c0c8",
            ClaimName = "BO.Device",
            DisplayName = "Quản lý thiết bị POS",
            IsDefault = false,
            IsActive = true,
            IsClaim = true,
            OrderIndex = index++,
            Type = EPermission.Web,
        });

        builder.HasData(new Permission {
            Id = "f86c9ffac8c3453db8d807f08c35c659",
            ParentId = "ec0f270b424249438540a16e9157c0c8",
            ClaimName = "BO.Report",
            DisplayName = "Báo cáo",
            IsDefault = false,
            IsActive = true,
            IsClaim = true,
            OrderIndex = index++,
            Type = EPermission.Web,
        });

        builder.HasData(new Permission {
            Id = "5c547c37abc14f4f883564fadd9c8e9f",
            ParentId = "ec0f270b424249438540a16e9157c0c8",
            ClaimName = "BO.Audit",
            DisplayName = "Lịch sử thao tác",
            IsDefault = false,
            IsActive = true,
            IsClaim = true,
            OrderIndex = index++,
            Type = EPermission.Web,
        });

        builder.HasData(new Permission {
            Id = "dc1c2ce584d74428b4e5241a5502787d",
            ParentId = "ec0f270b424249438540a16e9157c0c8",
            ClaimName = "BO.Setting",
            DisplayName = "Cài đặt",
            IsDefault = false,
            IsActive = true,
            IsClaim = false,
            OrderIndex = index++,
            Type = EPermission.Web,
        });

        builder.HasData(new Permission {
            Id = "b35cc06a567e420f8d0bda3426091048",
            ParentId = "dc1c2ce584d74428b4e5241a5502787d",
            ClaimName = "BO.General",
            DisplayName = "Cài đặt chung",
            IsDefault = false,
            IsActive = true,
            IsClaim = false,
            OrderIndex = index++,
            Type = EPermission.Web,
        });

        builder.HasData(new Permission {
            Id = "b6ff2fa707ce4de796166f6c6f258bd3",
            ParentId = "b35cc06a567e420f8d0bda3426091048",
            ClaimName = "BO.General.Order",
            DisplayName = "Cài đặt đơn hàng",
            IsDefault = false,
            IsActive = true,
            IsClaim = true,
            OrderIndex = index++,
            Type = EPermission.Web,
        });

        builder.HasData(new Permission {
            Id = "aac9b4aecc3a4a8f8ffba55f9de4c714",
            ParentId = "b35cc06a567e420f8d0bda3426091048",
            ClaimName = "BO.General.GenerateCode",
            DisplayName = "Cài đặt mã",
            IsDefault = false,
            IsActive = true,
            IsClaim = true,
            OrderIndex = index++,
            Type = EPermission.Web,
        });

        builder.HasData(new Permission {
            Id = "9890ae03a8e04e2380bbf1b67852a434",
            ParentId = "b35cc06a567e420f8d0bda3426091048",
            ClaimName = "BO.General.Template",
            DisplayName = "Cài đặt mẫu",
            IsDefault = false,
            IsActive = true,
            IsClaim = true,
            OrderIndex = index++,
            Type = EPermission.Web,
        });

        builder.HasData(new Permission {
            Id = "4212831939fb4863ae6b3b3a57596513",
            ParentId = "b35cc06a567e420f8d0bda3426091048",
            ClaimName = "BO.General.Api",
            DisplayName = "Cài đặt API",
            IsDefault = false,
            IsActive = true,
            IsClaim = true,
            OrderIndex = index++,
            Type = EPermission.Web,
        });

        builder.HasData(new Permission {
            Id = "744249c25fb049c1aed2d852c7e43e35",
            ParentId = "b35cc06a567e420f8d0bda3426091048",
            ClaimName = "BO.General.Email",
            DisplayName = "Cài đặt API",
            IsDefault = false,
            IsActive = true,
            IsClaim = true,
            OrderIndex = index++,
            Type = EPermission.Web,
        });

        builder.HasData(new Permission {
            Id = "721bb6697d4c4579abc649ed838443cd",
            ParentId = "b35cc06a567e420f8d0bda3426091048",
            ClaimName = "BO.General.Advanced",
            DisplayName = "Cài đặt nâng cao",
            IsDefault = false,
            IsActive = true,
            IsClaim = true,
            OrderIndex = index++,
            Type = EPermission.Web,
        });

        builder.HasData(new Permission {
            Id = "296285809bac481890a454ea8aed6af4",
            ParentId = "dc1c2ce584d74428b4e5241a5502787d",
            ClaimName = "BO.User",
            DisplayName = "Người dùng",
            IsDefault = false,
            IsActive = true,
            IsClaim = true,
            OrderIndex = index++,
            Type = EPermission.Web,
        }, new Permission {
            Id = "98873832ebcb4d9fb12e9b21a187f12c",
            ParentId = "296285809bac481890a454ea8aed6af4",
            ClaimName = "BO.User.Reset",
            DisplayName = "Đặt lại mật khẩu",
            IsDefault = false,
            IsActive = true,
            IsClaim = true,
            OrderIndex = index++,
            Type = EPermission.Web,
        });

        builder.HasData(new Permission {
            Id = "cb26c94262ab4863baa6c516edfde134",
            ParentId = "dc1c2ce584d74428b4e5241a5502787d",
            ClaimName = "BO.Role",
            DisplayName = "Phân quyền",
            IsDefault = false,
            IsActive = true,
            IsClaim = true,
            OrderIndex = index++,
            Type = EPermission.Web,
        });

        builder.HasData(new Permission {
            Id = "05a8a774fe354708bac21d995b625669",
            ParentId = "dc1c2ce584d74428b4e5241a5502787d",
            ClaimName = "BO.PaymentMethod",
            DisplayName = "Phương thức thanh toán",
            IsDefault = false,
            IsActive = true,
            IsClaim = true,
            OrderIndex = index++,
            Type = EPermission.Web,
        });

        builder.HasData(new Permission {
            Id = "a51564bb459040ed889784e1f2f38b3e",
            ClaimName = "POS",
            DisplayName = "POS",
            IsDefault = true,
            IsActive = true,
            IsClaim = false,
            OrderIndex = index++,
            Type = EPermission.POS,
        });

        builder.HasData(new Permission {
            Id = "834bd9886e504050bac964f95dfbf1d6",
            ParentId = "a51564bb459040ed889784e1f2f38b3e",
            ClaimName = "POS.Sale",
            DisplayName = "Bán hàng",
            IsDefault = true,
            IsActive = true,
            IsClaim = true,
            OrderIndex = index++,
            Type = EPermission.POS,
        });

        builder.HasData(new Permission {
            Id = "3dcc2d7c495447b8a5054793c1d7b99b",
            ParentId = "a51564bb459040ed889784e1f2f38b3e",
            ClaimName = "POS.Order",
            DisplayName = "Đơn hàng",
            IsDefault = true,
            IsActive = true,
            IsClaim = true,
            OrderIndex = index++,
            Type = EPermission.POS,
        });

        builder.HasData(new Permission {
            Id = "3dc6083590b74b45a5bf6ed63aaa1267",
            ParentId = "a51564bb459040ed889784e1f2f38b3e",
            ClaimName = "POS.Setting",
            DisplayName = "Cài đặt",
            IsDefault = false,
            IsActive = true,
            IsClaim = true,
            OrderIndex = index,
            Type = EPermission.POS,
        });
    }
}
