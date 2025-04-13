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
    /// Логика взаимодействия для SubjectsPage.xaml
    /// </summary>
    public partial class SubjectsPage : Page
    {
        private LibraryContext _context;
        public ObservableCollection<Subject> Subjects { get; set; }

        public SubjectsPage()
        {
            InitializeComponent();
            _context = new LibraryContext();
            LoadSubjects();
            DataContext = this;
        }

        private void LoadSubjects()
        {
            Subjects = new ObservableCollection<Subject>(_context.Subjects.ToList());
            SubjectGrid.ItemsSource = Subjects;
            OnPropertyChanged(nameof(Subjects));
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchBox.Text.ToLower();
            var result = _context.Subjects
                .Where(s => s.Name.ToLower().Contains(query))
                .ToList();

            Subjects = new ObservableCollection<Subject>(result);
            SubjectGrid.ItemsSource = Subjects;
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
            LoadSubjects();
        }

        private void AddSubject_Click(object sender, RoutedEventArgs e)
        {
            var subject = new Subject { Name = "Новая тематика" };
            _context.Subjects.Add(subject);
            _context.SaveChanges();
            LoadSubjects();
        }

        private void DeleteSubject_Click(object sender, RoutedEventArgs e)
        {
            if (SubjectGrid.SelectedItem is Subject selected)
            {
                try
                {
                    _context.Subjects.Remove(selected);
                    _context.SaveChanges();
                    LoadSubjects();
                }
                catch (DbUpdateException)
                {
                    MessageBox.Show("Нельзя удалить тематику, связанную с книгами.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            _context.SaveChanges();
            MessageBox.Show("Изменения сохранены.");
            LoadSubjects();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
