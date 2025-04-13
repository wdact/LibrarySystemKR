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
            var decreaseTrigger = @"
                DROP TRIGGER IF EXISTS trg_DecreaseBookQuantityOnSubscription;
                GO

                CREATE TRIGGER trg_DecreaseBookQuantityOnSubscription
                ON Subscriptions
                AFTER INSERT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    DECLARE @BookId INT, @LibraryId INT;

                    SELECT @BookId = BookId, @LibraryId = LibraryId FROM inserted;

                    IF EXISTS (
                        SELECT 1 FROM Books
                        WHERE BookId = @BookId AND LibraryId = @LibraryId AND Quantity > 0
                    )
                    BEGIN
                        UPDATE Books
                        SET Quantity = Quantity - 1
                        WHERE BookId = @BookId AND LibraryId = @LibraryId;
                    END
                    ELSE
                    BEGIN
                        ROLLBACK TRANSACTION;
                        RAISERROR ('Нет доступных экземпляров книги для подписки.', 16, 1);
                    END
                END;
            ";

            var increaseTrigger = @"
                DROP TRIGGER IF EXISTS trg_IncreaseBookQuantityOnUnsubscribe;
                GO

                CREATE TRIGGER trg_IncreaseBookQuantityOnUnsubscribe
                ON Subscriptions
                AFTER DELETE
                AS
                BEGIN
                    SET NOCOUNT ON;

                    UPDATE b
                    SET b.Quantity = b.Quantity + 1
                    FROM Books b
                    JOIN deleted d ON b.BookId = d.BookId AND b.LibraryId = d.LibraryId;
                END;
            ";

            migrationBuilder.Sql(decreaseTrigger);
            migrationBuilder.Sql(increaseTrigger);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_DecreaseBookQuantityOnSubscription");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_IncreaseBookQuantityOnUnsubscribe");
        }
    }
}
