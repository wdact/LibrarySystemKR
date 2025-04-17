using System.ComponentModel;
using System.Diagnostics;
using LibrarySystemKR.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystemKR.Helpres
{
    public static class DbHelper
    {
        public static int AddBookSmart(Book book)
        {
            using var context = new LibraryContext();
            using var command = context.Database.GetDbConnection().CreateCommand();

            command.CommandText = @"
        INSERT INTO Books (LibraryId, SubjectId, Author, Title, Publisher, PlaceOfPublication, YearOfPublication, Quantity)
        VALUES (@LibraryId, @SubjectId, @Author, @Title, @Publisher, @Place, @Year, @Quantity);
        SELECT CAST(SCOPE_IDENTITY() AS INT);";
            command.CommandType = System.Data.CommandType.Text;

            command.Parameters.Add(new SqlParameter("@LibraryId", book.LibraryId));
            command.Parameters.Add(new SqlParameter("@SubjectId", book.SubjectId));
            command.Parameters.Add(new SqlParameter("@Author", book.Author));
            command.Parameters.Add(new SqlParameter("@Title", book.Title));
            command.Parameters.Add(new SqlParameter("@Publisher", book.Publisher));
            command.Parameters.Add(new SqlParameter("@Place", book.PlaceOfPublication));
            command.Parameters.Add(new SqlParameter("@Year", book.YearOfPublication));
            command.Parameters.Add(new SqlParameter("@Quantity", book.Quantity));

            try
            {
                context.Database.OpenConnection();
                var result = command.ExecuteScalar();
                return Convert.ToInt32(result);
            }
            catch (SqlException ex)
            {
                Debug.WriteLine("Ошибка при добавлении книги в базу данных.\n\n" + ex.Message);
                throw;
            }
            finally
            {
                context.Database.CloseConnection();
            }
        }

        public static void AddSubscriptionSmart(Subscription subscription)
        {
            using var context = new LibraryContext();

            var sql = @"
                INSERT INTO Subscriptions (LibraryId, BookId, ReaderId, IssueDate, ReturnDate, Advance)
                VALUES (@LibraryId, @BookId, @ReaderId, @IssueDate, @ReturnDate, @Advance)";

            try
            {
                context.Database.ExecuteSqlRaw(sql,
                    new SqlParameter("@LibraryId", subscription.LibraryId),
                    new SqlParameter("@BookId", subscription.BookId),
                    new SqlParameter("@ReaderId", subscription.ReaderId),
                    new SqlParameter("@IssueDate", subscription.IssueDate),
                    new SqlParameter("@ReturnDate", subscription.ReturnDate),
                    new SqlParameter("@Advance", subscription.Advance)
                );
            }
            catch (SqlException ex)
            {
                Debug.WriteLine("Ошибка при добавлении подписки в базу данных.\n\n" + ex.Message);
                throw;
            }
        }

        public static void UpdateBookSmart(Book book)
        {
            using var context = new LibraryContext();

            var sql = @"
                UPDATE Books SET
                    SubjectId = @SubjectId,
                    Author = @Author,
                    Title = @Title,
                    Publisher = @Publisher,
                    PlaceOfPublication = @Place,
                    YearOfPublication = @Year,
                    Quantity = @Quantity
                WHERE BookId = @BookId";

            try
            {
                context.Database.ExecuteSqlRaw(sql,
                    new SqlParameter("@SubjectId", book.SubjectId),
                    new SqlParameter("@Author", book.Author),
                    new SqlParameter("@Title", book.Title),
                    new SqlParameter("@Publisher", book.Publisher),
                    new SqlParameter("@Place", book.PlaceOfPublication),
                    new SqlParameter("@Year", book.YearOfPublication),
                    new SqlParameter("@Quantity", book.Quantity),
                    new SqlParameter("@BookId", book.BookId)
                );
            }
            catch (SqlException ex)
            {
                Debug.WriteLine("Ошибка при обновлении книги.\n\n" + ex.Message);
                throw;
            }
        }

        public static void UpdateSubscriptionSmart(Subscription subscription)
        {
            using var context = new LibraryContext();

            var sql = @"
                UPDATE Subscriptions SET
                    IssueDate = @IssueDate,
                    ReturnDate = @ReturnDate,
                    Advance = @Advance
                WHERE LibraryId = @LibraryId AND BookId = @BookId AND ReaderId = @ReaderId";

            try
            {
                context.Database.ExecuteSqlRaw(sql,
                    new SqlParameter("@IssueDate", subscription.IssueDate),
                    new SqlParameter("@ReturnDate", subscription.ReturnDate),
                    new SqlParameter("@Advance", subscription.Advance),
                    new SqlParameter("@LibraryId", subscription.LibraryId),
                    new SqlParameter("@BookId", subscription.BookId),
                    new SqlParameter("@ReaderId", subscription.ReaderId)
                );
            }
            catch (SqlException ex)
            {
                Debug.WriteLine("Ошибка при обновлении подписки.\n\n" + ex.Message);
                throw;
            }
        }

        public static void DeleteBookSmart(int bookId)
        {
            using var context = new LibraryContext();

            var sql = @"
        DELETE FROM Books WHERE BookId = @BookId;
    ";

            try
            {
                context.Database.ExecuteSqlRaw(sql,
                    new SqlParameter("@BookId", bookId)
                );
            }
            catch (SqlException ex)
            {
                Debug.WriteLine("Ошибка при удалении книги:\n\n" + ex.Message);
                throw;
            }
        }

        public static void DeleteSubscriptionSmart(int libraryId, int bookId, int readerId)
        {
            using var context = new LibraryContext();

            var sql = @"
        DELETE FROM Subscriptions
        WHERE LibraryId = @LibraryId AND BookId = @BookId AND ReaderId = @ReaderId;
    ";

            try
            {
                context.Database.ExecuteSqlRaw(sql,
                    new SqlParameter("@LibraryId", libraryId),
                    new SqlParameter("@BookId", bookId),
                    new SqlParameter("@ReaderId", readerId)
                );
            }
            catch (SqlException ex)
            {
                Debug.WriteLine("Ошибка при удалении подписки:\n\n" + ex.Message);
                throw;
            }
        }
    }
}
