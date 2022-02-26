using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
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

        private void Image_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var wanted = (Image)sender;

            var imageAddress = wanted.Source.ToString().Split(new[] { "///" }, System.StringSplitOptions.RemoveEmptyEntries)[1];

            var newFile = imageAddress.Replace("retadata", "png");

            File.Copy(imageAddress, newFile, true);

            System.Diagnostics.Process.Start(newFile);
        }
    }
}
