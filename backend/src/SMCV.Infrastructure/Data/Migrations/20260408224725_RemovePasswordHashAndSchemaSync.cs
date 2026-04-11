using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMCV.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovePasswordHashAndSchemaSync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "varchar(500)",
                nullable: false,
                defaultValue: "");
        }
    }
}
