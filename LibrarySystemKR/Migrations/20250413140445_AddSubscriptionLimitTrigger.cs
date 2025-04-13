using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibrarySystemKR.Migrations
{
    /// <inheritdoc />
    public partial class AddSubscriptionLimitTrigger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sql = @"
                CREATE TRIGGER trg_CheckBookAvailability
                ON Subscriptions
                INSTEAD OF INSERT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    IF EXISTS (
                        SELECT 1
                        FROM inserted i
                        JOIN Books b ON i.BookId = b.BookId
                        WHERE (
                            SELECT COUNT(*)
                            FROM Subscriptions s
                            WHERE s.BookId = i.BookId
                              AND s.LibraryId = i.LibraryId
                              AND s.ReturnDate >= GETDATE()
                        ) >= b.Quantity
                    )
                    BEGIN
                        RAISERROR('Невозможно выдать книгу: все экземпляры уже выданы.', 16, 1);
                        RETURN;
                    END

                    INSERT INTO Subscriptions (LibraryId, BookId, ReaderId, IssueDate, ReturnDate, Advance)
                    SELECT LibraryId, BookId, ReaderId, IssueDate, ReturnDate, Advance
                    FROM inserted
                END
            ";

            migrationBuilder.Sql(sql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_CheckBookAvailability");
        }
    }
}
