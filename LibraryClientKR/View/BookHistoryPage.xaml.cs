using LibrarySystemKR.Helpres;
using LibrarySystemKR.Models;
using LibrarySystemKR;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using LibraryClientKR.Helpers;
using Microsoft.EntityFrameworkCore;

namespace LibraryClientKR.View
{
    /// <summary>
    /// Логика взаимодействия для BookHistoryPage.xaml
    /// </summary>
    public partial class BookHistoryPage : Page
    {
        private LibraryContext _context;
        private ObservableCollection<BookHistory> _history;

        public BookHistoryPage()
        {
            InitializeComponent();
            _context = new LibraryContext();
            LoadFilters();
            LoadData();
        }

        private void LoadFilters()
        {
            LibraryFilter.ItemsSource = _context.Libraries.ToList();
        }

        private void LoadData()
        {
            var query = _context.BookHistory
                .Include(h => h.Book)
                    .ThenInclude(b => b.Library)
                .AsQueryable();

            if (LibraryFilter.SelectedValue is int libraryId)
            {
                query = query.Where(h => h.Book.LibraryId == libraryId);
            }

            if (ActionFilter.SelectedItem is ComboBoxItem selectedAction && selectedAction.Content.ToString() != "Все действия")
            {
                query = query.Where(h => h.ActionType == selectedAction.Content.ToString());
            }

            if (!string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                string q = SearchBox.Text.ToLower();
                query = query.Where(h => h.Book.Title.ToLower().Contains(q));
            }

            _history = new ObservableCollection<BookHistory>(query.ToList());
            HistoryGrid.ItemsSource = _history;
        }

        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void ExportToPdf_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                FileName = "BookHistoryReport.pdf"
            };

            if (dialog.ShowDialog() == true)
            {
                ReportExporter.ExportBookHistoryToPdf(_history.ToList(), dialog.FileName);
            }
        }
    }
}
