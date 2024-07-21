using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tum4ik.JustClipboardManager.Migrations
{
    /// <inheritdoc />
    public partial class IsBuiltInForPlugin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBuiltIn",
                table: "Plugins",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBuiltIn",
                table: "Plugins");
        }
    }
}
