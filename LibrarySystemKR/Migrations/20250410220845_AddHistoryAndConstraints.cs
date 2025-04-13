using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibrarySystemKR.Migrations
{
    /// <inheritdoc />
    public partial class AddHistoryAndConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Создание таблицы BookHistory
            migrationBuilder.CreateTable(
                name: "BookHistory",
                columns: table => new
                {
                    HistoryId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LibraryId = table.Column<int>(nullable: false),
                    BookId = table.Column<int>(nullable: false),
                    ActionDate = table.Column<DateTime>(nullable: false),
                    ActionType = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookHistory", x => x.HistoryId);
                    // Внешний ключ на Book (составной ключ)
                    table.ForeignKey(
                        name: "FK_BookHistory_Books_LibraryId_BookId",
                        columns: x => new { x.LibraryId, x.BookId },
                        principalTable: "Books",
                        principalColumns: new[] { "LibraryId", "BookId" },
                        onDelete: ReferentialAction.Cascade);

                    // Внешний ключ на Library (NO ACTION)
                    table.ForeignKey(
                        name: "FK_BookHistory_Libraries_LibraryId",
                        column: x => x.LibraryId,
                        principalTable: "Libraries",
                        principalColumn: "LibraryId",
                        onDelete: ReferentialAction.NoAction);
                });

            // Индекс для BookId в таблице BookHistory
            migrationBuilder.CreateIndex(
                name: "IX_BookHistory_BookId",
                table: "BookHistory",
                column: "BookId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
