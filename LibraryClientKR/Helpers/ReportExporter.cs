using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibrarySystemKR.Models;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace LibraryClientKR.Helpers
{
    internal class ReportExporter
    {
        public static void ExportReaderSummaryToPdf(List<ReaderSubscriptionSummary> data)
        {
            var fileName = $"ReaderSummary_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

            Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(30);
                        page.Header().Text("Отчет по читателям").FontSize(20).Bold().AlignCenter();
                        page.Content().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background("#EEE").Padding(5).Text("Читатель").SemiBold();
                                header.Cell().Background("#EEE").Padding(5).Text("Подписок").SemiBold();
                            });

                            foreach (var item in data)
                            {
                                table.Cell().Padding(5).Text(item.FullName);
                                table.Cell().Padding(5).Text(item.SubscriptionCount.ToString());
                            }

                            table.Footer(footer =>
                            {
                                footer.Cell().ColumnSpan(2).AlignRight().PaddingTop(10)
                                    .Text($"Всего подписок: {data.Sum(x => x.SubscriptionCount)}").Bold();
                            });
                        });
                        page.Footer().AlignCenter().Text($"Сформировано: {DateTime.Now:g}").FontSize(10);
                    });
                })
                .GeneratePdf(fileName);

            Process.Start(new ProcessStartInfo(fileName) { UseShellExecute = true });
        }

        public static void ExportTopicSummaryToPdf(List<TopicSubscriptionSummary> data)
        {
            var fileName = $"TopicSummary_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

            Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(30);
                        page.Header().Text("Отчет по темам").FontSize(20).Bold().AlignCenter();

                        page.Content().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background("#EEE").Padding(5).Text("Тема").SemiBold();
                                header.Cell().Background("#EEE").Padding(5).Text("Подписок").SemiBold();
                            });

                            foreach (var item in data)
                            {
                                table.Cell().Padding(5).Text(item.Subject);
                                table.Cell().Padding(5).Text(item.SubscriptionCount.ToString());
                            }

                            table.Footer(footer =>
                            {
                                footer.Cell().ColumnSpan(2).AlignRight().PaddingTop(10)
                                    .Text($"Всего подписок: {data.Sum(x => x.SubscriptionCount)}").Bold();
                            });
                        });

                        page.Footer().AlignCenter().Text($"Сформировано: {DateTime.Now:g}").FontSize(10);
                    });
                })
                .GeneratePdf(fileName);

            Process.Start(new ProcessStartInfo(fileName) { UseShellExecute = true });
        }

        public static void ExportSubscriptionSummaryToPdf(List<SubscriptionSummary> data)
        {
            string fileName = $"SubscriptionSummary_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Text("Отчет по подпискам").FontSize(20).Bold().AlignCenter();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2); // Книга
                            columns.RelativeColumn(2); // Автор
                            columns.RelativeColumn(2); // Библиотека
                            columns.RelativeColumn(1); // Кол-во подписок
                            columns.RelativeColumn(2); // Выдано
                            columns.RelativeColumn(2); // Возврат
                        });

                        table.Header(header =>
                        {
                            string[] headers = { "Книга", "Автор", "Библиотека", "Кол-во", "Первое", "Последнее" };
                            foreach (var h in headers)
                                header.Cell().Background("#EEE").Padding(5).Text(h).SemiBold();
                        });

                        foreach (var item in data)
                        {
                            table.Cell().Padding(5).Text(item.BookTitle);
                            table.Cell().Padding(5).Text(item.Author);
                            table.Cell().Padding(5).Text(item.LibraryName);
                            table.Cell().Padding(5).Text(item.SubscriptionCount.ToString());
                            table.Cell().Padding(5).Text(item.FirstIssued.ToShortDateString());
                            table.Cell().Padding(5).Text(item.LastReturned.ToShortDateString());
                        }

                        table.Footer(footer =>
                        {
                            footer.Cell().ColumnSpan(6).AlignRight().PaddingTop(10)
                                .Text($"Итого подписок: {data.Sum(x => x.SubscriptionCount)}").Bold();
                        });
                    });

                    page.Footer().AlignCenter().Text($"Сформировано: {DateTime.Now:g}").FontSize(10);
                });
            })
            .GeneratePdf(fileName);

            Process.Start(new ProcessStartInfo(fileName) { UseShellExecute = true });
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
    }
}
