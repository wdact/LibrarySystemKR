using Microsoft.EntityFrameworkCore;
using LibrarySystemKR.Models;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibrarySystemKR
{
    public class LibraryContext : DbContext
    {
        public DbSet<Library> Libraries { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Reader> Readers { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<BookHistory> BookHistory { get; set; }
        public DbSet<SubscriptionSummary> ViewSubscriptionSummary { get; set; }
        public DbSet<TopicSubscriptionSummary> ViewTopicSubscriptionSummary { get; set; }
        public DbSet<ReaderSubscriptionSummary> ViewReaderSubscriptionSummary { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS02;Database=LibraryDB;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Добавляем аннотацию для максимальной длины идентификаторов
            modelBuilder.HasAnnotation("Relational:MaxIdentifierLength", 128);
            modelBuilder.HasAnnotation("ProductVersion", "9.0.4");

            // Таблица Library
            modelBuilder.Entity<Library>(entity =>
            {
                entity.HasKey(l => l.LibraryId);  // PRIMARY KEY для Library

                // DEFAULT значения
                entity.Property(l => l.Address)
                    .HasDefaultValue("Unknown Address");

                entity.Property(l => l.Name)
                    .HasDefaultValue("Unnamed Library");

                entity.HasIndex(l => l.Name)  // Индекс по имени библиотеки
                    .HasDatabaseName("IX_Library_Name");
            });

            // Таблица Book
            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(b => b.BookId);  // PRIMARY KEY для книги

                entity.Property(b => b.BookId)
                    .ValueGeneratedOnAdd()  // Указывает, что поле будет автоинкрементироваться
                    .UseIdentityColumn();  // Устанавливаем автоинкремент для столбца

                // Внешний ключ для Subject
                entity.HasOne(b => b.Subject)
                    .WithMany()
                    .HasForeignKey(b => b.SubjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Внешний ключ для Library
                entity.HasOne(b => b.Library)
                    .WithMany(l => l.Books)
                    .HasForeignKey(b => b.LibraryId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Индексы для часто запрашиваемых полей
                entity.HasIndex(b => b.SubjectId);  // Индекс для SubjectId

                // Значение по умолчанию
                entity.Property(b => b.Quantity)
                    .HasDefaultValue(0);
            });

            // Таблица Subject
            modelBuilder.Entity<Subject>(entity =>
            {
                entity.HasKey(s => s.SubjectId);  // PRIMARY KEY для Subject

                // Индексы для часто запрашиваемых полей
                entity.HasIndex(s => s.Name);  // Индекс по имени темы
            });

            // Таблица Reader
            modelBuilder.Entity<Reader>(entity =>
            {
                entity.HasKey(r => r.ReaderId);  // PRIMARY KEY для Reader

                // Индексы для часто запрашиваемых полей
                entity.HasIndex(r => r.FullName);  // Индекс по ФИО
            });

            // Таблица Subscription
            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.HasKey(s => new { s.LibraryId, s.BookId, s.ReaderId });  // Составной PRIMARY KEY

                // Внешний ключ для Reader
                entity.HasOne(s => s.Reader)
                    .WithMany()
                    .HasForeignKey(s => s.ReaderId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Внешний ключ для Library
                entity.HasOne(s => s.Library)
                    .WithMany()
                    .HasForeignKey(s => s.LibraryId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Внешний ключ для Book
                entity.HasOne(s => s.Book)
                    .WithMany()
                    .HasForeignKey(s => s.BookId )
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Настройка связи BookHistory с таблицей Books
            modelBuilder.Entity<BookHistory>()
                .HasKey(bh => bh.HistoryId);  // Указываем, что HistoryId — это PRIMARY KEY

            // Настроим составной внешний ключ
            modelBuilder.Entity<BookHistory>()
                .HasOne(bh => bh.Book)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SubscriptionSummary>()
                .HasNoKey()
                .ToView("View_SubscriptionSummary");

            modelBuilder.Entity<TopicSubscriptionSummary>()
                .HasNoKey()
                .ToView("View_TopicSubscriptionSummary");

            modelBuilder.Entity<ReaderSubscriptionSummary>()
                .HasNoKey()
                .ToView("View_ReaderSubscriptionSummary");
        }
    }
}
