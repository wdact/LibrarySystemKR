using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using LibraryClientKR.View;
using LibrarySystemKR;
using LibrarySystemKR.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem
{
    public partial class MainWindow : Window
    {
        private LibraryContext _context;

        public MainWindow()
        {
            InitializeComponent();
            _context = new LibraryContext();
        }

        private void Books_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new BookPage();
        }

        private void Readers_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ReaderPage();
        }

        private void Libraries_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new LibrariesPage();
        }

        private void Subjects_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new SubjectsPage();
        }

        private void Subscriptions_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new SubscriptionPage();
        }

        private void Reports_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new SubjectReportPage();
        }

        private void Reports_Click2(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new SubscriptionSummaryReportPage();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new BookHistoryPage();
        }

        private void ButtonBase_OnClick1(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new TopicReportPage();
        }

        private void ButtonBase_OnClick2(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ReaderReportPage();
        }
    }
}
