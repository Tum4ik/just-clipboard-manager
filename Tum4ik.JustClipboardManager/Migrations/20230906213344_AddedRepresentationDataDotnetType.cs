using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tum4ik.JustClipboardManager.Migrations
{
    /// <inheritdoc />
    public partial class AddedRepresentationDataDotnetType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RepresentationDataDotnetType",
                table: "Clips",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RepresentationDataDotnetType",
                table: "Clips");
        }
    }
}
