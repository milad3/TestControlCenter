using System.Collections;
using System.Collections.Generic;
using System.Windows;
using TestControlCenter.Models;
using TestControlCenter.Services;
using TestControlCenterDomain;

namespace TestControlCenter.Windows
{
    /// <summary>
    /// Interaction logic for AnswerRecordsViewerWindow.xaml
    /// </summary>
    public partial class AnswerRecordsViewerWindow : Window
    {
        private readonly IEnumerable<TestItemQuestionClueRecord> records;

        public AnswerRecordsViewerViewModel ViewModel { get; set; }

        public AnswerRecordsViewerWindow(IEnumerable<TestItemQuestionClueRecord> records)
        {
            InitializeComponent();
            this.records = records;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel = ViewModelsHelper.GetAnswerRecordsViewerViewModel(records);

            DataContext = ViewModel;
        }
    }
}
