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
using LibraryClientKR.Helpers;
using LibrarySystemKR.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryClientKR.View
{
    /// <summary>
    /// Логика взаимодействия для SubscriptionSummaryReportPage.xaml
    /// </summary>
    public partial class SubscriptionSummaryReportPage : Page
    {
        private readonly LibraryContext _context = new();
        private bool _isInitialized = false;

        public SubscriptionSummaryReportPage()
        {
            InitializeComponent();
            Loaded += SubscriptionSummaryReportPage_Loaded;
        }

        private void SubscriptionSummaryReportPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isInitialized) return;
            _isInitialized = true;
            LoadLibraries();
            LoadReport();
        }

        private void LoadLibraries()
        {
            var libs = _context.Libraries.Select(l => l.Name).ToList();
            LibraryFilterComboBox.ItemsSource = libs;
        }

        private void LoadReport()
        {
            if (ReportDataGrid == null) return;

            string selectedLibrary = LibraryFilterComboBox.SelectedItem?.ToString();
            int minCount = int.TryParse(MinCountTextBox.Text, out var mc) ? mc : 1;
            bool useSql = SourceSelector.SelectedIndex == 1;
            string authorFilter = AuthorFilterTextBox?.Text?.ToLower() ?? string.Empty;
            DateTime? startDate = StartDatePicker?.SelectedDate;
            DateTime? endDate = EndDatePicker?.SelectedDate;

            List<SubscriptionSummary> reportData;

            if (useSql)
            {
                reportData = _context.ViewSubscriptionSummary
                    .Where(s => (string.IsNullOrEmpty(selectedLibrary) || s.LibraryName == selectedLibrary)
                             && s.SubscriptionCount >= minCount
                             && (string.IsNullOrEmpty(authorFilter) || s.Author.ToLower().Contains(authorFilter))
                             && (!startDate.HasValue || s.FirstIssued >= startDate.Value)
                             && (!endDate.HasValue || s.LastReturned <= endDate.Value))
                    .OrderByDescending(s => s.SubscriptionCount)
                    .ToList();
            }
            else
            {
                reportData = _context.Subscriptions
                    .Include(s => s.Book).ThenInclude(b => b.Library)
                    .GroupBy(s => new { s.Book.Title, s.Book.Author, s.Book.Library.Name })
                    .Select(g => new SubscriptionSummary
                    {
                        BookTitle = g.Key.Title,
                        Author = g.Key.Author,
                        LibraryName = g.Key.Name,
                        SubscriptionCount = g.Count(),
                        FirstIssued = g.Min(s => s.IssueDate),
                        LastReturned = g.Max(s => s.ReturnDate)
                    })
                    .Where(s => (string.IsNullOrEmpty(selectedLibrary) || s.LibraryName == selectedLibrary)
                             && s.SubscriptionCount >= minCount
                             && (string.IsNullOrEmpty(authorFilter) || s.Author.ToLower().Contains(authorFilter))
                             && (!startDate.HasValue || s.FirstIssued >= startDate.Value)
                             && (!endDate.HasValue || s.LastReturned <= endDate.Value))
                    .OrderByDescending(s => s.SubscriptionCount)
                    .ToList();
            }

            ReportDataGrid.ItemsSource = reportData;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e) => LoadReport();
        private void SourceSelector_SelectionChanged(object sender, SelectionChangedEventArgs e) => LoadReport();

        private void ExportToPdf_Click(object sender, RoutedEventArgs e)
        {
            if (ReportDataGrid == null) return;

            var data = ReportDataGrid.ItemsSource as IEnumerable<SubscriptionSummary>;
            if (data is null || !data.Any())
            {
                MessageBox.Show("Нет данных для экспорта.");
                return;
            }

            try
            {
                ReportExporter.ExportSubscriptionSummaryToPdf(data.ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта: {ex.Message}");
            }
        }

        private void ClearFilters_Click(object sender, RoutedEventArgs e)
        {
            LibraryFilterComboBox.SelectedItem = null;
            MinCountTextBox.Text = string.Empty;
            AuthorFilterTextBox.Text = string.Empty;
            StartDatePicker.SelectedDate = null;
            EndDatePicker.SelectedDate = null;
            LoadReport();
        }
    }
}
