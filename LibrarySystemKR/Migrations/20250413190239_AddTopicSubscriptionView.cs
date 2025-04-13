using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibrarySystemKR.Migrations
{
    /// <inheritdoc />
    public partial class AddTopicSubscriptionView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR ALTER VIEW View_TopicSubscriptionSummary AS
                SELECT 
                    s.Name AS Subject,
                    COUNT(*) AS SubscriptionCount
                FROM Subscriptions sub
                JOIN Books b ON sub.BookId = b.BookId
                JOIN Subjects s ON b.SubjectId = s.SubjectId
                GROUP BY s.Name
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS View_TopicSubscriptionSummary");
        }
    }
}
