using System.Linq;
using System.Windows;
using TestControlCenter.Models;
using TestControlCenter.Services;
using TestControlCenter.Tools;
using TestControlCenterDomain;

namespace TestControlCenter.Windows
{
    /// <summary>
    /// Interaction logic for ExamDetailsWindow.xaml
    /// </summary>
    public partial class ExamDetailsWindow : Window
    {
        private readonly TestMark testMark;

        public TestMarkingViewModel ViewModel { get; set; }

        public ExamDetailsWindow(TestMark testMark)
        {
            InitializeComponent();
            this.testMark = testMark;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel = await ViewModelsHelper.GetExamDetailsViewModel(testMark);

            DataContext = ViewModel;
        }

        private void SequenceViewButton_Click(object sender, RoutedEventArgs e)
        {
            var elem = (FrameworkElement)sender;
            if (!(elem.DataContext is TestMarkAnswer data))
            {
                return;
            }

            if (data?.Records == null)
            {
                NotificationsHelper.Error("موردی برای نمایش وجود ندارد.", "خطا");
                return;
            }

            var window = new AnswerRecordsViewerWindow(data.Records)
            {
                Owner = this
            };
            window.Show();
        }

        private void AnswerViewButton_Click(object sender, RoutedEventArgs e)
        {
            var elem = (FrameworkElement)sender;
            if (!(elem.DataContext is TestMarkAnswer data))
            {
                return;
            }

            if(data?.Records == null)
            {
                NotificationsHelper.Error("موردی برای نمایش وجود ندارد.", "خطا");
                return;
            }

            var window = new AnswerRecordsViewerWindow(data.Records.Where(x => x.RecordType == TestItemQuestionClueRecordType.Answer))
            {
                Owner = this
            };
            window.Show();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            ButtonsContainer.IsEnabled = false;

            var result = await ViewModel.Mark(Application.Current.Dispatcher);

            if (!result)
            {
                NotificationsHelper.Warning("امکان تصحیح اتوماتیک وجود ندارد.", "یافت نشد!");
            }

            ButtonsContainer.IsEnabled = true;
        }
    }
}
