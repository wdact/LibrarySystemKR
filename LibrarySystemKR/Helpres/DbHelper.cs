using System.ComponentModel;
using System.Diagnostics;
using LibrarySystemKR.Models;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace LibrarySystemKR.Helpres
{
    public static class DbHelper
    {
        public static int AddBookSmart(LibraryContext context, Book book)
        {
            try
            {
                context.Books.Add(book);
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message.Contains("triggers") == true)
                {
                    Debug.WriteLine("Обнаружены триггеры. Переключаемся на прямую вставку...", "Инфо");

                    var sql = @"
                        DECLARE @InsertedIds TABLE (BookId INT);
                        INSERT INTO Books (LibraryId, SubjectId, Author, Title, Publisher, PlaceOfPublication, YearOfPublication, Quantity)
                        OUTPUT INSERTED.BookId INTO @InsertedIds
                        VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7});
                        SELECT BookId FROM @InsertedIds;";

                    var id = context.Database.ExecuteSqlRaw(sql,
                        book.LibraryId,
                        book.SubjectId,
                        book.Author,
                        book.Title,
                        book.Publisher,
                        book.PlaceOfPublication,
                        book.YearOfPublication,
                        book.Quantity);

                    Debug.WriteLine("Книга успешно добавлена вручную через SQL.");

                    return id;
                }
            }

            return 0;
        }

        public static void AddSubscriptionSmart(LibraryContext context, Subscription subscription)
        {
            try
            {
                context.Subscriptions.Add(subscription);
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message.Contains("triggers") == true)
                {
                    Debug.WriteLine("Ошибка сохранения подписки из-за триггеров. Выполняется SQL-вставка...", "Инфо");

                    context.Database.ExecuteSqlRaw(@"
                        INSERT INTO Subscriptions (LibraryId, BookId, ReaderId, IssueDate, ReturnDate, Advance)
                        VALUES ({0}, {1}, {2}, {3}, {4}, {5})",
                        subscription.LibraryId,
                        subscription.BookId,
                        subscription.ReaderId,
                        subscription.IssueDate,
                        subscription.ReturnDate,
                        subscription.Advance);

                    Debug.WriteLine("Подписка успешно добавлена вручную через SQL.");
                }
                else
                {
                    throw;
                }
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

        public static void ExportBooksBySubjectReport(LibraryContext context, string filePath, int minQuantity = 0, int? yearFrom = null, int? yearTo = null, int? subjectId = null, string titleContains = null)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var query = context.Books.Include(b => b.Subject).AsQueryable();

            if (yearFrom.HasValue)
                query = query.Where(b => b.YearOfPublication >= yearFrom);

            if (yearTo.HasValue)
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
