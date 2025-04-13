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
    /// Логика взаимодействия для ReaderReportPage.xaml
    /// </summary>
    public partial class ReaderReportPage : Page
    {
        private readonly LibraryContext _context = new();
        private bool _isInitialized = false;

        public ReaderReportPage()
        {
            InitializeComponent();
            Loaded += ReaderReportPage_Loaded;
        }

        private void ReaderReportPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isInitialized) return;
            _isInitialized = true;
            LoadReport();
        }

        private void LoadReport()
        {
            if (ReportDataGrid == null) return;

            string search = SearchBox.Text.ToLower();
            int minCount = int.TryParse(MinCountTextBox.Text, out var mc) ? mc : 1;
            bool useSql = SourceSelector.SelectedIndex == 1;

            List<ReaderSubscriptionSummary> reportData;

            if (useSql)
            {
                reportData = _context.ViewReaderSubscriptionSummary
                    .Where(r => r.SubscriptionCount >= minCount)
                    .Where(r => string.IsNullOrEmpty(search) || r.FullName.ToLower().Contains(search))
                    .ToList();
            }
            else
            {
                reportData = _context.Subscriptions
                    .Include(s => s.Reader)
                    .GroupBy(s => s.Reader.FullName)
                    .Select(g => new ReaderSubscriptionSummary
                    {
                        FullName = g.Key,
                        SubscriptionCount = g.Count()
                    })
                    .Where(r => r.SubscriptionCount >= minCount)
                    .Where(r => string.IsNullOrEmpty(search) || r.FullName.ToLower().Contains(search))
                    .OrderByDescending(r => r.SubscriptionCount)
                    .ToList();
            }

            ReportDataGrid.ItemsSource = reportData;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e) => LoadReport();
        private void SourceSelector_SelectionChanged(object sender, SelectionChangedEventArgs e) => LoadReport();

        private void ClearFilters_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = string.Empty;
            MinCountTextBox.Text = string.Empty;
            LoadReport();
        }

        private void ExportToPdf_Click(object sender, RoutedEventArgs e)
        {
            if (ReportDataGrid?.ItemsSource is not IEnumerable<ReaderSubscriptionSummary> data || !data.Any())
            {
                MessageBox.Show("Нет данных для экспорта.");
                return;
            }

            try
            {
                //ReportExporter.ExportReaderSummaryToPdf(data.ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта: {ex.Message}");
            }
        }
    }
}
