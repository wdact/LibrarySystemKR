using LibrarySystemKR.Helpres;
using LibrarySystemKR;
using Microsoft.Win32;
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

namespace LibraryClientKR.View
{
    /// <summary>
    /// Логика взаимодействия для SubjectReportPage.xaml
    /// </summary>
    public partial class SubjectReportPage : Page
    {
        private readonly LibraryContext _context = new();

        private enum ReportType
        {
            BooksBySubject,
            SubscriptionsByReader,
            SubscriptionsByLibrary
        }

        private TextBox minQuantityBox;
        private TextBox minAdvanceBox;
        private TextBox minCountBox;
        private DatePicker fromDatePicker;
        private DatePicker toDatePicker;
        private ComboBox subjectComboBox;
        private TextBox libraryNameBox;
        private ComboBox readerComboBox;
        private ComboBox libraryComboBox;
        private TextBox titleFilterBox;
        private TextBox yearFromBox;
        private TextBox yearToBox;

        public SubjectReportPage()
        {
            InitializeComponent();
            ReportTypeComboBox.ItemsSource = Enum.GetValues(typeof(ReportType));
            ReportTypeComboBox.SelectedIndex = 0;
        }

        private void ReportTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FiltersPanel.Children.Clear();

            switch ((ReportType)ReportTypeComboBox.SelectedItem)
            {
                case ReportType.BooksBySubject:
                    FiltersPanel.Children.Add(new Label { Content = "Минимум книг:" });
                    minQuantityBox = new TextBox { Text = "0", Width = 100 };
                    FiltersPanel.Children.Add(minQuantityBox);

                    FiltersPanel.Children.Add(new Label { Content = "Год от:" });
                    yearFromBox = new TextBox { Width = 100 };
                    FiltersPanel.Children.Add(yearFromBox);

                    FiltersPanel.Children.Add(new Label { Content = "Год до:" });
                    yearToBox = new TextBox { Width = 100 };
                    FiltersPanel.Children.Add(yearToBox);

                    FiltersPanel.Children.Add(new Label { Content = "Тематика:" });
                    subjectComboBox = new ComboBox
                    {
                        Width = 200,
                        DisplayMemberPath = "Name",
                        SelectedValuePath = "SubjectId",
                        ItemsSource = _context.Subjects.ToList()
                    };
                    FiltersPanel.Children.Add(subjectComboBox);

                    FiltersPanel.Children.Add(new Label { Content = "Название книги содержит:" });
                    titleFilterBox = new TextBox { Width = 200 };
                    FiltersPanel.Children.Add(titleFilterBox);
                    break;

                case ReportType.SubscriptionsByReader:
                    FiltersPanel.Children.Add(new Label { Content = "Минимум аванса:" });
                    minAdvanceBox = new TextBox { Text = "0", Width = 100 };
                    FiltersPanel.Children.Add(minAdvanceBox);

                    FiltersPanel.Children.Add(new Label { Content = "Начало периода:" });
                    fromDatePicker = new DatePicker { Width = 150 };
                    FiltersPanel.Children.Add(fromDatePicker);

                    FiltersPanel.Children.Add(new Label { Content = "Конец периода:" });
                    toDatePicker = new DatePicker { Width = 150 };
                    FiltersPanel.Children.Add(toDatePicker);

                    FiltersPanel.Children.Add(new Label { Content = "Читатель:" });
                    readerComboBox = new ComboBox
                    {
                        Width = 200,
                        DisplayMemberPath = "FullName",
                        SelectedValuePath = "ReaderId",
                        ItemsSource = _context.Readers.ToList()
                    };
                    FiltersPanel.Children.Add(readerComboBox);

                    FiltersPanel.Children.Add(new Label { Content = "Библиотека:" });
                    libraryComboBox = new ComboBox
                    {
                        Width = 200,
                        DisplayMemberPath = "Name",
                        SelectedValuePath = "LibraryId",
                        ItemsSource = _context.Libraries.ToList()
                    };
                    FiltersPanel.Children.Add(libraryComboBox);
                    break;

                case ReportType.SubscriptionsByLibrary:
                    FiltersPanel.Children.Add(new Label { Content = "Мин. кол-во подписок:" });
                    minCountBox = new TextBox { Text = "0", Width = 100 };
                    FiltersPanel.Children.Add(minCountBox);

                    FiltersPanel.Children.Add(new Label { Content = "Начало периода:" });
                    fromDatePicker = new DatePicker { Width = 150 };
                    FiltersPanel.Children.Add(fromDatePicker);

                    FiltersPanel.Children.Add(new Label { Content = "Конец периода:" });
                    toDatePicker = new DatePicker { Width = 150 };
                    FiltersPanel.Children.Add(toDatePicker);

                    FiltersPanel.Children.Add(new Label { Content = "Тематика:" });
                    subjectComboBox = new ComboBox
                    {
                        Width = 200,
                        DisplayMemberPath = "Name",
                        SelectedValuePath = "SubjectId",
                        ItemsSource = _context.Subjects.ToList()
                    };
                    FiltersPanel.Children.Add(subjectComboBox);

                    FiltersPanel.Children.Add(new Label { Content = "Название библиотеки содержит:" });
                    libraryNameBox = new TextBox { Width = 150 };
                    FiltersPanel.Children.Add(libraryNameBox);
                    break;
            }
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "PDF файл (*.pdf)|*.pdf",
                FileName = $"Report_{DateTime.Now:yyyyMMdd_HHmm}.pdf"
            };

            if (dialog.ShowDialog() != true)
                return;

            switch ((ReportType)ReportTypeComboBox.SelectedItem)
            {
                case ReportType.BooksBySubject:
                    int.TryParse(minQuantityBox.Text, out int minQ);
                    int.TryParse(yearFromBox.Text, out int yFrom);
                    int.TryParse(yearToBox.Text, out int yTo);
                    var sid = subjectComboBox.SelectedValue as int?;
                    var title = string.IsNullOrWhiteSpace(titleFilterBox.Text) ? null : titleFilterBox.Text;
                    DbHelper.ExportBooksBySubjectReport(_context, dialog.FileName, minQ, yFrom, yTo, sid, title);
                    break;

                case ReportType.SubscriptionsByReader:
                    decimal.TryParse(minAdvanceBox.Text, out decimal minA);
                    var from = fromDatePicker.SelectedDate;
                    var to = toDatePicker.SelectedDate;
                    var readerId = readerComboBox.SelectedValue as int?;
                    var libId = libraryComboBox.SelectedValue as int?;
                    DbHelper.ExportSubscriptionsByReaderReport(_context, dialog.FileName, minA, from, to, readerId, libId);
                    break;

                case ReportType.SubscriptionsByLibrary:
                    int.TryParse(minCountBox.Text, out int minC);
                    var f = fromDatePicker.SelectedDate;
                    var t = toDatePicker.SelectedDate;
                    var subjId = subjectComboBox.SelectedValue as int?;
                    var libName = string.IsNullOrWhiteSpace(libraryNameBox.Text) ? null : libraryNameBox.Text;
                    DbHelper.ExportSubscriptionsByLibraryReport(_context, dialog.FileName, minC, f, t, subjId, libName);
                    break;
            }
        }
    }
}
