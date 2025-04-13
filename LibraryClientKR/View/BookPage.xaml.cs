using LibrarySystemKR.Models;
using LibrarySystemKR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Reflection.Metadata.BlobBuilder;
using System.Collections.ObjectModel;
using System.ComponentModel;
using LibraryClientKR.Helpers;
using LibrarySystemKR.Helpres;
using Microsoft.EntityFrameworkCore;

namespace LibraryClientKR.View
{
    public partial class BookPage : Page, INotifyPropertyChanged
    {
        private LibraryContext _context;

        public static readonly DependencyProperty LibrariesProperty = DependencyProperty.Register(
            nameof(Libraries), typeof(ObservableCollection<Library>), typeof(BookPage), new PropertyMetadata(default(ObservableCollection<Library>)));

        public ObservableCollection<Library> Libraries
        {
            get { return (ObservableCollection<Library>)GetValue(LibrariesProperty); }
            set { SetValue(LibrariesProperty, value); }
        }

        public static readonly DependencyProperty SubjectsProperty = DependencyProperty.Register(
            nameof(Subjects), typeof(ObservableCollection<Subject>), typeof(BookPage), new PropertyMetadata(default(ObservableCollection<Subject>)));

        public ObservableCollection<Subject> Subjects
        {
            get { return (ObservableCollection<Subject>)GetValue(SubjectsProperty); }
            set { SetValue(SubjectsProperty, value); }
        }

        private string _currentSortColumn = null;
        private ListSortDirection _currentSortDirection = ListSortDirection.Ascending;

        public ObservableCollection<Book> Books { get; set; }

        public BookPage()
        {
            InitializeComponent();
            _context = new LibraryContext();
            LoadData();
            DataContext = this;
        }

        private void LoadData()
        {
            Libraries = new ObservableCollection<Library>(_context.Libraries.ToList());
            Subjects = new ObservableCollection<Subject>(_context.Subjects.ToList());
            Books = new ObservableCollection<Book>(
                _context.Books
                    .Include(b => b.Library)
                    .Include(b => b.Subject)
                    .ToList()
            );

            FilterLibraryComboBox.ItemsSource = Libraries;
            FilterSubjectComboBox.ItemsSource = Subjects;

            OnPropertyChanged(nameof(Libraries));
            OnPropertyChanged(nameof(Subjects));
            BookGrid.ItemsSource = Books;
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchBox.Text.ToLower();
            var result = _context.Books
                .Where(b => b.Title.ToLower().Contains(query))
                .ToList();

            Books = new ObservableCollection<Book>(result);
            BookGrid.ItemsSource = Books;
        }

        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            var newBook = new Book
            {
                Title = "Новая книга",
                Author = "Автор",
                Publisher = "Издательство",
                PlaceOfPublication = "Город",
                YearOfPublication = 2025,
                Quantity = 1,
                LibraryId = Libraries.FirstOrDefault()?.LibraryId ?? 1,
                SubjectId = Subjects.FirstOrDefault()?.SubjectId ?? 1
            };

            DbHelper.AddBookSmart(newBook);
            LoadData();
        }

        private void DeleteBook_Click(object sender, RoutedEventArgs e)
        {
            if (BookGrid.SelectedItem is Book selected)
            {
                DbHelper.DeleteBookSmart(selected.BookId);
                LoadData();
            }
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            foreach (var book in Books)
            {
                try
                {
                    DbHelper.UpdateBookSmart(book);
                }
                catch { /* ошибки уже обрабатываются внутри */ }
            }
            MessageBox.Show("Изменения сохранены.");
            LoadData();
        }

        private void LoadBooks()
        {
            var query = _context.Books
                .Include(b => b.Library)
                .Include(b => b.Subject)
                .AsQueryable();

            // фильтры
            if (FilterLibraryComboBox.SelectedValue is int libraryId)
                query = query.Where(b => b.LibraryId == libraryId);

            if (FilterSubjectComboBox.SelectedValue is int subjectId)
                query = query.Where(b => b.SubjectId == subjectId);

            // сортировка
            if (!string.IsNullOrEmpty(_currentSortColumn))
            {
                query = _currentSortDirection == ListSortDirection.Ascending
                    ? query.OrderByDynamic(_currentSortColumn)
                    : query.OrderByDescendingDynamic(_currentSortColumn);
            }

            Books = new ObservableCollection<Book>(query.ToList());
            BookGrid.ItemsSource = Books;
        }


        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            LoadBooks();
        }

        private void ClearFilter_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
            FilterLibraryComboBox.SelectedItem = null;
            FilterSubjectComboBox.SelectedItem = null;
            LoadBooks();
        }

        private void BookGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            e.Handled = true; // отключаем авто-сортировку WPF

            string column = e.Column.SortMemberPath;

            if (_currentSortColumn == column)
            {
                // Если снова кликнули по тому же столбцу — меняем направление
                _currentSortDirection = _currentSortDirection == ListSortDirection.Ascending
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;
            }
            else
            {
                _currentSortColumn = column;
                _currentSortDirection = ListSortDirection.Ascending;
            }

            LoadBooks(); // перезагружаем книги с сортировкой

            // Обновляем визуальную стрелку
            foreach (var col in BookGrid.Columns)
                col.SortDirection = null;

            e.Column.SortDirection = _currentSortDirection;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}