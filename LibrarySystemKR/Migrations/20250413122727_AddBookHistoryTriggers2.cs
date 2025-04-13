using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibrarySystemKR.Migrations
{
    /// <inheritdoc />
    public partial class AddBookHistoryTriggers2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TRIGGER AfterBookInsert
                ON Books
                AFTER INSERT
                AS
                BEGIN
                    INSERT INTO BookHistory (LibraryId, BookId, ActionDate, ActionType)
                    SELECT LibraryId, BookId, GETDATE(), 'Добавление' FROM inserted;
                END
            ");

            migrationBuilder.Sql(@"
                CREATE TRIGGER AfterBookUpdate
                ON Books
                AFTER UPDATE
                AS
                BEGIN
                    INSERT INTO BookHistory (LibraryId, BookId, ActionDate, ActionType)
                    SELECT LibraryId, BookId, GETDATE(), 'Редактирование' FROM inserted;
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS AfterBookInsert;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS AfterBookUpdate;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS AfterBookDelete;");
        }
    }
}
