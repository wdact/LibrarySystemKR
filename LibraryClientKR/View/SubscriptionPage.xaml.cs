using LibrarySystemKR.Models;
using LibrarySystemKR;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using Microsoft.EntityFrameworkCore;

namespace LibraryClientKR.View
{
    /// <summary>
    /// Логика взаимодействия для SubscriptionPage.xaml
    /// </summary>
    public partial class SubscriptionPage : Page
    {
        private LibraryContext _context;

        public ObservableCollection<Subscription> Subscriptions { get; set; }
        public ObservableCollection<Book> Books { get; set; }
        public ObservableCollection<Reader> Readers { get; set; }
        public ObservableCollection<Library> Libraries { get; set; }

        public SubscriptionPage()
        {
            InitializeComponent();
            _context = new LibraryContext();
            LoadData();
            DataContext = this;
        }

        private void LoadData()
        {
            Readers = new ObservableCollection<Reader>(_context.Readers.ToList());
            Books = new ObservableCollection<Book>(_context.Books.Include(b => b.Library).ToList());
            Libraries = new ObservableCollection<Library>(_context.Libraries.ToList());

            LibraryFilterComboBox.ItemsSource = Libraries;

            LoadSubscriptions();
            OnPropertyChanged(nameof(Readers));
            OnPropertyChanged(nameof(Books));
        }

        private void LoadSubscriptions()
        {
            var query = _context.Subscriptions
                .Include(s => s.Book)
                .Include(s => s.Reader)
                .AsQueryable();

            if (LibraryFilterComboBox.SelectedValue is int libraryId)
                query = query.Where(s => s.LibraryId == libraryId);

            string q = SearchBox.Text.ToLower();
            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(s =>
                    s.Reader.FullName.ToLower().Contains(q) ||
                    s.Book.Title.ToLower().Contains(q));
            }

            Subscriptions = new ObservableCollection<Subscription>(query.ToList());
            SubscriptionGrid.ItemsSource = Subscriptions;
        }

        private void Search_Click(object sender, RoutedEventArgs e) => LoadSubscriptions();
        private void ClearFilter_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
            LibraryFilterComboBox.SelectedItem = null;
            LoadSubscriptions();
        }

        private void AddSubscription_Click(object sender, RoutedEventArgs e)
        {
            if (Books.Count == 0 || Readers.Count == 0)
            {
                MessageBox.Show("Сначала добавьте книги и читателей.");
                return;
            }

            var firstBook = Books.First();
            var firstReader = Readers.First();

            var newSub = new Subscription
            {
                LibraryId = firstBook.LibraryId,
                BookId = firstBook.BookId,
                ReaderId = firstReader.ReaderId,
                IssueDate = DateTime.Today,
                ReturnDate = DateTime.Today.AddDays(14),
                Advance = 0
            };

            _context.Subscriptions.Add(newSub);
            _context.SaveChanges();
            LoadSubscriptions();
        }

        private void DeleteSubscription_Click(object sender, RoutedEventArgs e)
        {
            if (SubscriptionGrid.SelectedItem is Subscription selected)
            {
                _context.Subscriptions.Remove(selected);
                _context.SaveChanges();
                LoadSubscriptions();
            }
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            foreach (var sub in Subscriptions)
            {
                if (sub.ReturnDate < sub.IssueDate)
                {
                    MessageBox.Show("Дата возврата не может быть раньше даты выдачи.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            _context.SaveChanges();
            MessageBox.Show("Изменения сохранены.");
            LoadSubscriptions();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
