using LibrarySystemKR.Models;
using LibrarySystemKR;
using Microsoft.EntityFrameworkCore;
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
    /// Логика взаимодействия для LibrariesPage.xaml
    /// </summary>
    public partial class LibrariesPage : Page
    {
        private LibraryContext _context;
        public ObservableCollection<Library> Libraries { get; set; }

        public LibrariesPage()
        {
            InitializeComponent();
            _context = new LibraryContext();
            LoadLibraries();
            DataContext = this;
        }

        private void LoadLibraries()
        {
            Libraries = new ObservableCollection<Library>(_context.Libraries.ToList());
            LibraryGrid.ItemsSource = Libraries;
            OnPropertyChanged(nameof(Libraries));
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchBox.Text.ToLower();

            var result = _context.Libraries
                .Where(l =>
                    l.Name.ToLower().Contains(query) ||
                    l.Address.ToLower().Contains(query))
                .ToList();

            Libraries = new ObservableCollection<Library>(result);
            LibraryGrid.ItemsSource = Libraries;
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
            LoadLibraries();
        }

        private void AddLibrary_Click(object sender, RoutedEventArgs e)
        {
            var lib = new Library
            {
                Name = "Новая библиотека",
                Address = "Адрес",
                LastUpdated = DateTime.Now
            };

            _context.Libraries.Add(lib);
            _context.SaveChanges();
            LoadLibraries();
        }

        private void DeleteLibrary_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryGrid.SelectedItem is Library selected)
            {
                try
                {
                    _context.Libraries.Remove(selected);
                    _context.SaveChanges();
                    LoadLibraries();
                }
                catch (DbUpdateException)
                {
                    MessageBox.Show("Нельзя удалить библиотеку, содержащую книги.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            foreach (var lib in Libraries)
            {
                lib.LastUpdated = DateTime.Now;
            }

            _context.SaveChanges();
            MessageBox.Show("Изменения сохранены.");
            LoadLibraries();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
