using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibrarySystemKR.Migrations
{
    /// <inheritdoc />
    public partial class AddBookHistoryTriggers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var insertTrigger = @"
                CREATE TRIGGER trg_SubscriptionInsert
                ON Subscriptions
                AFTER INSERT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    INSERT INTO BookHistory (BookId, LibraryId, ActionDate, ActionType)
                    SELECT BookId, LibraryId, GETDATE(), 'Выдана'
                    FROM inserted;
                END
            ";

            var returnTrigger = @"
                CREATE TRIGGER trg_SubscriptionUpdate
                ON Subscriptions
                AFTER UPDATE
                AS
                BEGIN
                    SET NOCOUNT ON;

                    INSERT INTO BookHistory (BookId, LibraryId, ActionDate, ActionType)
                    SELECT i.BookId, i.LibraryId, GETDATE(), 'Возвращена'
                    FROM inserted i
                    JOIN deleted d ON i.LibraryId = d.LibraryId AND i.BookId = d.BookId AND i.ReaderId = d.ReaderId
                    WHERE d.ReturnDate < i.ReturnDate;
                END
            ";

            migrationBuilder.Sql(insertTrigger);
            migrationBuilder.Sql(returnTrigger);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_SubscriptionInsert");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_SubscriptionUpdate");
        }
    }
}
