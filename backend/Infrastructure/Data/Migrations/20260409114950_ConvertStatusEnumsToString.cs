using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMCV.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ConvertStatusEnumsToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EmailStatus",
                table: "Contacts",
                type: "text",
                nullable: false,
                defaultValue: "Pending",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Campaigns",
                type: "text",
                nullable: false,
                defaultValue: "Draft",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "EmailStatus",
                table: "Contacts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "Pending");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Campaigns",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "Draft");
        }
    }
}
