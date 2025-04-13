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
using LibrarySystemKR.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryClientKR.View
{
    /// <summary>
    /// Логика взаимодействия для TopicReportPage.xaml
    /// </summary>
    public partial class TopicReportPage : Page
    {
        private readonly LibraryContext _context = new();
        private bool _isInitialized = false;

        public TopicReportPage()
        {
            InitializeComponent();
            Loaded += TopicReportPage_Loaded;
        }

        private void TopicReportPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isInitialized) return;
            _isInitialized = true;
            LoadLibraries();
            SourceSelector.SelectedIndex = 0;
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

            List<TopicSubscriptionSummary> reportData;

            if (useSql)
            {
                reportData = _context.ViewTopicSubscriptionSummary
                    .Where(r => r.SubscriptionCount >= minCount)
                    .Where(r => string.IsNullOrEmpty(selectedLibrary) || _context.Books.Any(b => b.Subject.Name == r.Subject && b.Library.Name == selectedLibrary))
                    .ToList();
            }
            else
            {
                reportData = _context.Subscriptions
                    .Include(s => s.Book).ThenInclude(b => b.Subject)
                    .Include(s => s.Book.Library)
                    .Where(s => string.IsNullOrEmpty(selectedLibrary) || s.Book.Library.Name == selectedLibrary)
                    .GroupBy(s => s.Book.Subject.Name)
                    .Select(g => new TopicSubscriptionSummary
                    {
                        Subject = g.Key,
                        SubscriptionCount = g.Count()
                    })
                    .Where(r => r.SubscriptionCount >= minCount)
                    .OrderByDescending(r => r.SubscriptionCount)
                    .ToList();
            }

            ReportDataGrid.ItemsSource = reportData;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e) => LoadReport();

        private void SourceSelector_SelectionChanged(object sender, SelectionChangedEventArgs e) => LoadReport();

        private void ClearFilters_Click(object sender, RoutedEventArgs e)
        {
            LibraryFilterComboBox.SelectedItem = null;
            MinCountTextBox.Text = string.Empty;
            LoadReport();
        }

        private void ExportToPdf_Click(object sender, RoutedEventArgs e)
        {
            if (ReportDataGrid?.ItemsSource is not IEnumerable<TopicSubscriptionSummary> data || !data.Any())
            {
                MessageBox.Show("Нет данных для экспорта.");
                return;
            }

            try
            {
                //ReportExporter.ExportTopicSummaryToPdf(data.ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта: {ex.Message}");
            }
        }
    }
}
