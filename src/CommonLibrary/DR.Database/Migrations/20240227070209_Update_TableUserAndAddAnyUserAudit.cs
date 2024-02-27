using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DR.Database.Migrations
{
    /// <inheritdoc />
    public partial class Update_TableUserAndAddAnyUserAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GeneralSetting",
                schema: "public",
                columns: table => new
                {
                    Code = table.Column<int>(type: "integer", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DefaultValue = table.Column<string>(type: "text", nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralSetting", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "UserAudit",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    MerchantId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Action = table.Column<int>(type: "integer", nullable: false),
                    DocType = table.Column<int>(type: "integer", nullable: false),
                    DocId = table.Column<string>(type: "text", nullable: true),
                    DocCode = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    Time = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAudit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAudit_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MerchantSetting",
                schema: "public",
                columns: table => new
                {
                    MerchantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<int>(type: "integer", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantSetting", x => new { x.MerchantId, x.Code });
                    table.ForeignKey(
                        name: "FK_MerchantSetting_GeneralSetting_Code",
                        column: x => x.Code,
                        principalSchema: "public",
                        principalTable: "GeneralSetting",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "GeneralSetting",
                columns: new[] { "Code", "DefaultValue", "Description", "DisplayName", "OrderIndex" },
                values: new object[,]
                {
                    { 101, "{\"Prefix\":\"SP\",\"StartNumber\":1,\"IncrNumber\":1,\"IsRandomIncrNumber\":false,\"ResetBy\":null}", "Tiền tố mã sản phẩm khi được tạo tự động.", "Tiền tố mã sản phẩm", 0 },
                    { 102, "{\"Prefix\":\"KH\",\"StartNumber\":1,\"IncrNumber\":1,\"IsRandomIncrNumber\":false,\"ResetBy\":null}", "Tiền tố mã khách hàng khi được tạo tự động.", "Tiền tố mã khách hàng", 3 },
                    { 103, "{\"Prefix\":\"NCC\",\"StartNumber\":1,\"IncrNumber\":1,\"IsRandomIncrNumber\":false,\"ResetBy\":null}", "Tiền tố mã nhà cung cấp khi được tạo tự động.", "Tiền tố mã nhà cung cấp", 6 },
                    { 104, "{\"Prefix\":\"CH\",\"StartNumber\":1,\"IncrNumber\":1,\"IsRandomIncrNumber\":false,\"ResetBy\":null}", "Tiền tố mã cửa hàng khi được tạo tự động", "Tiền tố mã cửa hàng", 13 },
                    { 105, "{\"Prefix\":\"K\",\"StartNumber\":1,\"IncrNumber\":1,\"IsRandomIncrNumber\":false,\"ResetBy\":null}", "Tiền tố mã kho khi được tạo tự động.", "Tiền tố mã kho", 7 },
                    { 106, "{\"Prefix\":\"PQ\",\"StartNumber\":1,\"IncrNumber\":1,\"IsRandomIncrNumber\":false,\"ResetBy\":null}", "Tiền tố mã phần quyền khi được tạo tự động.", "Tiền tố mã phần quyền", 14 },
                    { 107, "{\"Prefix\":\"NK\",\"StartNumber\":1,\"IncrNumber\":1,\"IsRandomIncrNumber\":false,\"ResetBy\":3}", "Tiền tố mã phiếu nhập kho khi được tạo tự động.", "Tiền tố mã phiếu nhập kho", 8 },
                    { 108, "{\"Prefix\":\"XK\",\"StartNumber\":1,\"IncrNumber\":1,\"IsRandomIncrNumber\":false,\"ResetBy\":1}", "Tiền tố mã phiếu xuất kho khi được tạo tự động.", "Tiền tố mã phiếu xuất kho", 9 },
                    { 109, "{\"Prefix\":\"CK\",\"StartNumber\":1,\"IncrNumber\":1,\"IsRandomIncrNumber\":false,\"ResetBy\":3}", "Tiền tố mã phiếu chuyển kho khi được tạo tự động.", "Tiền tố mã phiếu chuyển kho", 10 },
                    { 110, "{\"Prefix\":\"TH\",\"StartNumber\":1,\"IncrNumber\":1,\"IsRandomIncrNumber\":false,\"ResetBy\":3}", "Tiền tố mã phiếu trả hàng khi được tạo tự động.", "Tiền tố mã phiếu trả hàng", 11 },
                    { 111, "{\"Prefix\":\"ĐH\",\"StartNumber\":1,\"IncrNumber\":1,\"IsRandomIncrNumber\":false,\"ResetBy\":1}", "Tiền tố mã đơn hàng khi được tạo tự động.", "Tiền tố mã đơn hàng", 5 },
                    { 112, "{\"Prefix\":\"DM\",\"StartNumber\":1,\"IncrNumber\":1,\"IsRandomIncrNumber\":false,\"ResetBy\":null}", "Tiền tố mã danh mục khi được tạo tự động.", "Tiền tố mã danh mục", 1 },
                    { 113, "{\"Prefix\":\"KK\",\"StartNumber\":1,\"IncrNumber\":1,\"IsRandomIncrNumber\":false,\"ResetBy\":4}", "Tiền tố mã phiếu kiểm kho khi được tạo tự động.", "Tiền tố mã phiếu kiểm kho", 12 },
                    { 114, "{\"Prefix\":\"PTTT\",\"StartNumber\":1,\"IncrNumber\":1,\"IsRandomIncrNumber\":false,\"ResetBy\":null}", "Tiền tố mã phương thức thanh toán khi được tạo tự động.", "Tiền tố mã phương thức thanh toán", 15 },
                    { 115, "{\"Hour\":0,\"Day\":false,\"Week\":false,\"Month\":false,\"IsSend\":false,\"Emails\":[]}", "Báo cáo doanh thu ngày/tuần/tháng qua email.", "Báo cáo qua email", 16 },
                    { 116, "{\"Prefix\":\"PT\",\"StartNumber\":1,\"IncrNumber\":1,\"IsRandomIncrNumber\":false,\"ResetBy\":1}", "Tiền tố mã phiếu thu khi được tạo tự động.", "Tiền tố mã phiếu thu", 17 },
                    { 117, "{\"Prefix\":\"PC\",\"StartNumber\":1,\"IncrNumber\":1,\"IsRandomIncrNumber\":false,\"ResetBy\":1}", "Tiền tố mã phiếu chi khi được tạo tự động.", "Tiền tố mã phiếu chi", 18 },
                    { 118, "{\"Prefix\":\"NKH\",\"StartNumber\":1,\"IncrNumber\":1,\"IsRandomIncrNumber\":false,\"ResetBy\":null}", "Tiền tố mã nhóm khách hàng khi được tạo tự động.", "Tiền tố mã nhóm khách hàng", 4 },
                    { 119, "{\"Prefix\":\"ThH\",\"StartNumber\":1,\"IncrNumber\":1,\"IsRandomIncrNumber\":false,\"ResetBy\":null}", "Tiền tố mã thương hiệu khi được tạo tự động.", "Tiền tố mã thương hiệu", 2 },
                    { 200, "false", "Tự động cập nhật số lượng xuất khi tạo hoặc sửa đơn hàng.", "Tự động xuất khi tạo đơn hàng", 19 },
                    { 201, "{\"ThousandsSeparator\":\",\",\"DecimalSeparator\":\".\",\"NumberDecimalDigitsForQuantity\":1,\"NumberDecimalDigitsForCurrency\":0}", "Định dạng phần nghìn và phần thập phân của số.", "Định dạng số", 20 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MerchantSetting_Code",
                schema: "public",
                table: "MerchantSetting",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_UserAudit_MerchantId",
                schema: "public",
                table: "UserAudit",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAudit_MerchantId_UserId",
                schema: "public",
                table: "UserAudit",
                columns: new[] { "MerchantId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserAudit_UserId",
                schema: "public",
                table: "UserAudit",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MerchantSetting",
                schema: "public");

            migrationBuilder.DropTable(
                name: "UserAudit",
                schema: "public");

            migrationBuilder.DropTable(
                name: "GeneralSetting",
                schema: "public");
        }
    }
}
