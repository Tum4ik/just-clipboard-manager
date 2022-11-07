using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tum4ik.JustClipboardManager.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClipType = table.Column<int>(type: "INTEGER", nullable: false),
                    RepresentationData = table.Column<byte[]>(type: "BLOB", nullable: true),
                    SearchLabel = table.Column<string>(type: "TEXT", nullable: true),
                    ClippedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'localtime')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clips", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormattedDataObjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data = table.Column<byte[]>(type: "BLOB", nullable: false),
                    DataDotnetType = table.Column<string>(type: "TEXT", nullable: false),
                    Format = table.Column<string>(type: "TEXT", nullable: false),
                    FormatOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    ClipId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormattedDataObjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormattedDataObjects_Clips_ClipId",
                        column: x => x.ClipId,
                        principalTable: "Clips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clips_SearchLabel",
                table: "Clips",
                column: "SearchLabel");

            migrationBuilder.CreateIndex(
                name: "IX_FormattedDataObjects_ClipId",
                table: "FormattedDataObjects",
                column: "ClipId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormattedDataObjects");

            migrationBuilder.DropTable(
                name: "Clips");
        }
    }
}
