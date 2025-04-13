using LibrarySystemKR.Models;
using System;

namespace LibrarySystemKR
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new LibraryContext())
            {
                // 1. Добавляем библиотеку
                var library = new Library
                {
                    Name = "Central Library 2",
                    Address = "123 Main St"
                };
                context.Libraries.Add(library);
                context.SaveChanges();  // Сначала сохраняем библиотеку, чтобы получить значение для LibraryCode

                // 2. Добавляем тему (Subjects) - сохраняем её перед добавлением книги
                var subject = new Subject
                {
                    Name = "Fiction 2"  // Оставляем без явного значения для SubjectCode (автоинкремент)
                };
                context.Subjects.Add(subject);
                context.SaveChanges();  // Сначала сохраняем тему, чтобы получить значение для SubjectCode

                // Проверим, что значение SubjectCode было сгенерировано
                Console.WriteLine($"SubjectCode: {subject.SubjectId}");  // Выведем для проверки

                // 3. Добавляем читателя
                var reader = new Reader
                {
                    FullName = "Alice Smith",
                    Address = "456 Oak St",
                    Phone = "123-456-7890"
                };
                context.Readers.Add(reader);
                context.SaveChanges();  // Сначала сохраняем читателя, чтобы получить значение для ReaderCode

                // 4. Добавляем книгу, ссылаясь на библиотеку и тему
                var book = new Book
                {
                    LibraryId = library.LibraryId,  // Используем значение, полученное после сохранения библиотеки
                    SubjectId = subject.SubjectId,  // Используем значение, полученное после сохранения темы
                    Author = "John Doe",
                    Title = "The Great Adventure",
                    Publisher = "ABC Publishing",
                    PlaceOfPublication = "New York",
                    YearOfPublication = 2021,
                    Quantity = 10
                };
                context.Books.Add(book);
                context.SaveChanges();  // Сохраняем книгу, чтобы она была связана с библиотекой и темой

                // 5. Добавляем подписку
                var subscription = new Subscription
                {
                    BookId = book.BookId,  // Используем значение для BookCode
                    ReaderId = reader.ReaderId,  // Используем значение для ReaderCode
                    LibraryId = library.LibraryId,
                    IssueDate = DateTime.Now,
                    ReturnDate = DateTime.Now.AddDays(14),
                    Advance = 5.00m
                };
                context.Subscriptions.Add(subscription);
                context.SaveChanges();  // Сохраняем подписку
            }

            Console.WriteLine("Данные успешно добавлены в базу данных!");
        }
    }
}
