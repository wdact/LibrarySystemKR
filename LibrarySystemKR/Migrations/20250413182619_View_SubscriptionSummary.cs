using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibrarySystemKR.Migrations
{
    /// <inheritdoc />
    public partial class View_SubscriptionSummary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR ALTER VIEW View_SubscriptionSummary AS
                SELECT 
                    b.Title AS BookTitle,
                    b.Author,
                    l.Name AS LibraryName,
                    COUNT(*) AS SubscriptionCount,
                    MIN(s.IssueDate) AS FirstIssued,
                    MAX(s.ReturnDate) AS LastReturned
                FROM Subscriptions s
                JOIN Books b ON s.BookId = b.BookId
                JOIN Libraries l ON s.LibraryId = l.LibraryId
                GROUP BY b.Title, b.Author, l.Name
                HAVING COUNT(*) >= 1;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS View_SubscriptionSummary;");
        }
    }
}
