using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tum4ik.JustClipboardManager.Migrations
{
    /// <inheritdoc />
    public partial class ReplacedClipTypeByPluginId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClipType",
                table: "Clips");

            migrationBuilder.AlterColumn<byte[]>(
                name: "RepresentationData",
                table: "Clips",
                type: "BLOB",
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "BLOB",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PluginId",
                table: "Clips",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PluginId",
                table: "Clips");

            migrationBuilder.AlterColumn<byte[]>(
                name: "RepresentationData",
                table: "Clips",
                type: "BLOB",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "BLOB");

            migrationBuilder.AddColumn<int>(
                name: "ClipType",
                table: "Clips",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
