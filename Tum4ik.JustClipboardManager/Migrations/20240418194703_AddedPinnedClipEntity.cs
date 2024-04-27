using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tum4ik.JustClipboardManager.Migrations
{
    /// <inheritdoc />
    public partial class AddedPinnedClipEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PinnedClips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    ClipId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PinnedClips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PinnedClips_Clips_ClipId",
                        column: x => x.ClipId,
                        principalTable: "Clips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PinnedClips_ClipId",
                table: "PinnedClips",
                column: "ClipId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PinnedClips");
        }
    }
}
