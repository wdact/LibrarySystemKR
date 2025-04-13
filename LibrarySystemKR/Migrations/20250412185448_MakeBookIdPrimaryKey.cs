using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibrarySystemKR.Migrations
{
    /// <inheritdoc />
    public partial class MakeBookIdPrimaryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookHistory_Books_LibraryId_BookId",
                table: "BookHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Books_LibraryId_BookId",
                table: "Subscriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Books",
                table: "Books");

            migrationBuilder.AlterColumn<int>(
                name: "BookId",
                table: "Books",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Books",
                table: "Books",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_BookId",
                table: "Subscriptions",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_LibraryId",
                table: "Books",
                column: "LibraryId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookHistory_Books_BookId",
                table: "BookHistory",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Books_BookId",
                table: "Subscriptions",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookHistory_Books_BookId",
                table: "BookHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Books_BookId",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_BookId",
                table: "Subscriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Books",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_LibraryId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_BookHistory_BookId",
                table: "BookHistory");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "Libraries");

            migrationBuilder.AlterColumn<int>(
                name: "BookId",
                table: "Books",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Books",
                table: "Books",
                columns: new[] { "LibraryId", "BookId" });

            migrationBuilder.CreateIndex(
                name: "IX_BookHistory_LibraryId_BookId",
                table: "BookHistory",
                columns: new[] { "LibraryId", "BookId" });

            migrationBuilder.AddForeignKey(
                name: "FK_BookHistory_Books_LibraryId_BookId",
                table: "BookHistory",
                columns: new[] { "LibraryId", "BookId" },
                principalTable: "Books",
                principalColumns: new[] { "LibraryId", "BookId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Books_LibraryId_BookId",
                table: "Subscriptions",
                columns: new[] { "LibraryId", "BookId" },
                principalTable: "Books",
                principalColumns: new[] { "LibraryId", "BookId" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
