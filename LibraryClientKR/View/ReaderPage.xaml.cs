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

namespace LibraryClientKR.View
{
    /// <summary>
    /// Логика взаимодействия для ReaderPage.xaml
    /// </summary>
    public partial class ReaderPage : Page
    {
        private LibraryContext _context;

        public ObservableCollection<Reader> Readers { get; set; }

        public ReaderPage()
        {
            InitializeComponent();
            _context = new LibraryContext();
            LoadReaders();
            DataContext = this;
        }

        private void LoadReaders()
        {
            Readers = new ObservableCollection<Reader>(_context.Readers.ToList());
            ReaderGrid.ItemsSource = Readers;
            OnPropertyChanged(nameof(Readers));
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchBox.Text.ToLower();

            var results = _context.Readers
                .Where(r =>
                    r.FullName.ToLower().Contains(query) ||
                    r.Address.ToLower().Contains(query) ||
                    r.Phone.ToLower().Contains(query))
                .ToList();

            Readers = new ObservableCollection<Reader>(results);
            ReaderGrid.ItemsSource = Readers;
        }

        private void AddReader_Click(object sender, RoutedEventArgs e)
        {
            var newReader = new Reader
            {
                FullName = "Новый читатель",
                Address = "Адрес",
                Phone = "000-000-0000"
            };

            _context.Readers.Add(newReader);
            _context.SaveChanges();
            LoadReaders();
        }

        private void DeleteReader_Click(object sender, RoutedEventArgs e)
        {
            if (ReaderGrid.SelectedItem is Reader selected)
            {
                var confirm = MessageBox.Show($"Удалить читателя {selected.FullName}?", "Подтверждение", MessageBoxButton.YesNo);
                if (confirm == MessageBoxResult.Yes)
                {
                    _context.Readers.Remove(selected);
                    _context.SaveChanges();
                    LoadReaders();
                }
            }
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            _context.SaveChanges();
            MessageBox.Show("Изменения сохранены.");
            LoadReaders();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
