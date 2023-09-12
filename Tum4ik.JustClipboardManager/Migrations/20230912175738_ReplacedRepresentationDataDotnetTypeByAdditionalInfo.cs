using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tum4ik.JustClipboardManager.Migrations
{
    /// <inheritdoc />
    public partial class ReplacedRepresentationDataDotnetTypeByAdditionalInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RepresentationDataDotnetType",
                table: "Clips");

            migrationBuilder.AddColumn<string>(
                name: "AdditionalInfo",
                table: "Clips",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalInfo",
                table: "Clips");

            migrationBuilder.AddColumn<string>(
                name: "RepresentationDataDotnetType",
                table: "Clips",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
