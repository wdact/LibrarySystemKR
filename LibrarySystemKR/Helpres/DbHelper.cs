using System.ComponentModel;
using System.Diagnostics;
using LibrarySystemKR.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

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


        static TextStyle CellStyle => TextStyle.Default.Size(12).FontFamily("Arial");

        public static void ExportBookHistoryToPdf(List<BookHistory> history, string filePath)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Header().Text("История изменений книг").FontSize(20).SemiBold().AlignCenter();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(); // Книга
                            columns.RelativeColumn(); // Библиотека
                            columns.ConstantColumn(100); // Дата
                            columns.ConstantColumn(100); // Тип
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Книга").Style(CellStyle);
                            header.Cell().Text("Библиотека").Style(CellStyle);
                            header.Cell().Text("Дата").Style(CellStyle);
                            header.Cell().Text("Действие").Style(CellStyle);
                        });

                        foreach (var record in history)
                        {
                            table.Cell().Text(record.Book?.Title ?? "").Style(CellStyle);
                            table.Cell().Text(record.Book?.Library?.Name ?? "").Style(CellStyle);
                            table.Cell().Text(record.ActionDate.ToShortDateString()).Style(CellStyle);
                            table.Cell().Text(record.ActionType).Style(CellStyle);
                        }
                    });
                });
            })
            .GeneratePdf(filePath);

            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
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


        public static void ExportBooksBySubjectReport(LibraryContext context, string filePath, int minQuantity = 0, int? yearFrom = null, int? yearTo = null, int? subjectId = null, string titleContains = null)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var query = context.Books.Include(b => b.Subject).AsQueryable();

            if (yearFrom is > 0)
                query = query.Where(b => b.YearOfPublication >= yearFrom);

            if (yearTo is > 0)
                query = query.Where(b => b.YearOfPublication <= yearTo);

            if (subjectId.HasValue)
                query = query.Where(b => b.SubjectId == subjectId);

            if (!string.IsNullOrWhiteSpace(titleContains))
                query = query.Where(b => b.Title.Contains(titleContains));

            var data = query
                .GroupBy(b => b.Subject.Name)
                .Select(g => new
                {
                    Subject = g.Key,
                    TotalBooks = g.Sum(b => b.Quantity)
                })
                .Where(g => g.TotalBooks >= minQuantity)
                .OrderByDescending(g => g.TotalBooks)
                .ToList();

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Text("Отчёт: Количество книг по темам").FontSize(18).Bold().AlignCenter();
                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.ConstantColumn(100);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Тематика").Style(CellStyle);
                            header.Cell().Text("Количество").Style(CellStyle);
                        });

                        foreach (var row in data)
                        {
                            table.Cell().Text(row.Subject).Style(CellStyle);
                            table.Cell().Text(row.TotalBooks.ToString()).Style(CellStyle);
                        }
                    });

                    page.Footer().AlignRight().Text($"Итого тем: {data.Count}");
                });
            }).GeneratePdf(filePath);
            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
        }

        public static void ExportSubscriptionsByReaderReport(LibraryContext context, string filePath, decimal minAdvance = 0, DateTime? fromDate = null, DateTime? toDate = null, int? readerId = null, int? libraryId = null)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var query = context.Subscriptions
                .Include(s => s.Reader)
                .Include(s => s.Book)
                .AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(s => s.IssueDate >= fromDate);

            if (toDate.HasValue)
                query = query.Where(s => s.IssueDate <= toDate);

            if (readerId.HasValue)
                query = query.Where(s => s.ReaderId == readerId);

            if (libraryId.HasValue)
                query = query.Where(s => s.LibraryId == libraryId);

            var data = query
                .GroupBy(s => s.Reader.FullName)
                .Select(g => new
                {
                    Reader = g.Key,
                    TotalSubscriptions = g.Count(),
                    TotalAdvance = g.Sum(s => s.Advance)
                })
                .Where(g => g.TotalAdvance >= minAdvance)
                .OrderByDescending(g => g.TotalAdvance)
                .ToList();

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Text("Отчёт: Подписки по читателям").FontSize(18).Bold().AlignCenter();
                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.ConstantColumn(100);
                            columns.ConstantColumn(100);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Читатель").Style(CellStyle);
                            header.Cell().Text("Кол-во").Style(CellStyle);
                            header.Cell().Text("Аванс").Style(CellStyle);
                        });

                        foreach (var row in data)
                        {
                            table.Cell().Text(row.Reader).Style(CellStyle);
                            table.Cell().Text(row.TotalSubscriptions.ToString()).Style(CellStyle);
                            table.Cell().Text(row.TotalAdvance.ToString("F2")).Style(CellStyle);
                        }
                    });

                    page.Footer().AlignRight().Text($"Всего читателей: {data.Count}");
                });
            }).GeneratePdf(filePath);
            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
        }

        public static void ExportSubscriptionsByLibraryReport(
            LibraryContext context,
            string filePath,
            int minCount = 0,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int? subjectId = null,
            string libraryNameContains = null)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var query = context.Subscriptions
                .Include(s => s.Book)
                    .ThenInclude(b => b.Library)
                .Include(s => s.Book)
                    .ThenInclude(b => b.Subject)
                .AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(s => s.IssueDate >= fromDate);

            if (toDate.HasValue)
                query = query.Where(s => s.IssueDate <= toDate);

            if (subjectId.HasValue)
                query = query.Where(s => s.Book.SubjectId == subjectId);

            if (!string.IsNullOrWhiteSpace(libraryNameContains))
                query = query.Where(s => s.Book.Library.Name.Contains(libraryNameContains));

            var data = query
                .GroupBy(s => s.Book.Library.Name)
                .Select(g => new
                {
                    Library = g.Key,
                    TotalBooksIssued = g.Count(),
                    TotalAdvance = g.Sum(s => s.Advance)
                })
                .Where(g => g.TotalBooksIssued >= minCount)
                .OrderByDescending(g => g.TotalBooksIssued)
                .ToList();

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Text("Отчёт: Подписки по библиотекам").FontSize(18).Bold().AlignCenter();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(); // Название
                            columns.ConstantColumn(100); // Кол-во
                            columns.ConstantColumn(100); // Аванс
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Библиотека").Style(CellStyle);
                            header.Cell().Text("Выдано книг").Style(CellStyle);
                            header.Cell().Text("Сумма аванса").Style(CellStyle);
                        });

                        foreach (var row in data)
                        {
                            table.Cell().Text(row.Library).Style(CellStyle);
                            table.Cell().Text(row.TotalBooksIssued.ToString()).Style(CellStyle);
                            table.Cell().Text(row.TotalAdvance.ToString("F2")).Style(CellStyle);
                        }
                    });

                    page.Footer().AlignRight().Text($"Всего библиотек: {data.Count}");
                });
            }).GeneratePdf(filePath);

            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
        }
    }
}
