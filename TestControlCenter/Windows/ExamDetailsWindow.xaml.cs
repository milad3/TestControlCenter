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

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ViewModel.TestMark.Score = ViewModel.TestMark.TestMarkAnswers.Sum(x => x.PublicScore);
            ScoreTextBox.Text = ViewModel.TestMark.Score.ToString();

            await ViewModel.SaveResults();
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            await ViewModel.SaveResults();

            NotificationsHelper.Information("اطلاعات با موفقیت ذخیره شد.", "ذخیره");
        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            await ViewModel.FinilizeResults();

            NotificationsHelper.Information("وضعیت آزمون به نهایی تغییر کرد.", "توجه");
        }

        private async void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (ViewModel.TestMark.IsSynced)
            {
                NotificationsHelper.Warning("اطلاعات آزمون قبلا ارسال و ذخیره شده است.", "توجه");
                return;
            }

            SyncButton.IsEnabled = false;
            var temp = SyncButton.Content;
            SyncButton.Content = "لطفا منتظر باشید...";

            var result = await ViewModel.Sync();
            if(result)
            {
                NotificationsHelper.Information("اطلاعات با موفقیت ارسال شد.", "سرور");
            }
            else
            {
                NotificationsHelper.Error("در ارسال اطلاعات به سرور خطایی پیش آمد", "خطا");
            }

            SyncButton.Content = temp;
            SyncButton.IsEnabled = true;
        }
    }
}
