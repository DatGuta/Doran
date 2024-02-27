using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DR.Database.Migrations
{
    /// <inheritdoc />
    public partial class Change_UserAndPerMission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                schema: "public",
                table: "User");

            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                schema: "public",
                table: "User");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                schema: "public",
                table: "User",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "public",
                table: "Merchant",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(36)",
                oldMaxLength: 36);

            migrationBuilder.AddColumn<string>(
                name: "ApiSecret",
                schema: "public",
                table: "Merchant",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "At",
                schema: "public",
                table: "Merchant",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                schema: "public",
                table: "Merchant",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "ExpiredDate",
                schema: "public",
                table: "Merchant",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "public",
                table: "Merchant",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Merchant_Code",
                schema: "public",
                table: "Merchant",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Merchant_Code",
                schema: "public",
                table: "Merchant");

            migrationBuilder.DropColumn(
                name: "Password",
                schema: "public",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ApiSecret",
                schema: "public",
                table: "Merchant");

            migrationBuilder.DropColumn(
                name: "At",
                schema: "public",
                table: "Merchant");

            migrationBuilder.DropColumn(
                name: "Code",
                schema: "public",
                table: "Merchant");

            migrationBuilder.DropColumn(
                name: "ExpiredDate",
                schema: "public",
                table: "Merchant");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "public",
                table: "Merchant");

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordHash",
                schema: "public",
                table: "User",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordSalt",
                schema: "public",
                table: "User",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "public",
                table: "Merchant",
                type: "character varying(36)",
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);
        }
    }
}
