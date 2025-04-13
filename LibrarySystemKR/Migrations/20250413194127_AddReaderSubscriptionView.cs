using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibrarySystemKR.Migrations
{
    /// <inheritdoc />
    public partial class AddReaderSubscriptionView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR ALTER VIEW View_ReaderSubscriptionSummary AS
                SELECT 
                    r.FullName,
                    COUNT(*) AS SubscriptionCount
                FROM Subscriptions sub
                JOIN Readers r ON sub.ReaderId = r.ReaderId
                GROUP BY r.FullName
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS View_ReaderSubscriptionSummary");
        }
    }
}
