using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JustClipboardManager.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SearchLabelIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Clips_SearchLabel",
                table: "Clips",
                column: "SearchLabel");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Clips_SearchLabel",
                table: "Clips");
        }
    }
}
