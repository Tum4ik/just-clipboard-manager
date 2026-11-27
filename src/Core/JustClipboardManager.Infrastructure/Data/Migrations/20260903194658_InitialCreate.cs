using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JustClipboardManager.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clips",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PreviewData = table.Column<byte[]>(type: "BLOB", nullable: false),
                    SearchLabel = table.Column<string>(type: "TEXT", nullable: true),
                    ClippedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'localtime')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clips", x => x.Id);
                },
                comment: "Represents a single item in the clipboard history.");

            migrationBuilder.CreateTable(
                name: "ClipDataObjects",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FormatId = table.Column<int>(type: "INTEGER", nullable: false),
                    Data = table.Column<byte[]>(type: "BLOB", nullable: false),
                    ClipId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClipDataObjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClipDataObjects_Clips_ClipId",
                        column: x => x.ClipId,
                        principalTable: "Clips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Contains a clipboard item data for a specific format.");

            migrationBuilder.CreateIndex(
                name: "IX_ClipDataObjects_ClipId",
                table: "ClipDataObjects",
                column: "ClipId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClipDataObjects");

            migrationBuilder.DropTable(
                name: "Clips");
        }
    }
}
