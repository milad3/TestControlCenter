using System;
using System.Windows;
using TestControlCenterDomain;
using TestControlCenter.Models;
using TestControlCenter.Services;
using TestControlCenter.Infrastructure;
using TestControlCenter.Tools;
using System.Threading.Tasks;

namespace TestControlCenter.Windows
{
    /// <summary>
    /// Interaction logic for ExamWindow.xaml
    /// </summary>
    public partial class ExamWindow : Window
    {
        private readonly DateTime startDateTime;

        public delegate void ExamEndedEventHandler(object sender, EventArgs e);
        public event ExamEndedEventHandler ExamEndedEvent;

        public ExamViewModel ViewModel { get; set; }
        public bool IsFinished { get; private set; }

        public ExamWindow(TestItem testItem, MftStudent student)
        {
            InitializeComponent();

            ViewModel = ViewModelsHelper.GetExamViewModel(testItem, student, SystemParameters.WorkArea);
            ViewModel.TimesUpEvent += ViewModel_TimesUpEvent;

            ViewModel.TopMost = true;

            GlobalValues.ExamIsRunning = true;

            GlobalValues.Student = student;

            GlobalValues.Test = testItem;

            EventHookConfig.ExamTools = new ExamTools();

            startDateTime = DateTime.Now;

            var imagesDir = GlobalTools.GetImagesDir(testItem);
            GlobalValues.TestMarker = GlobalTools.LoadTestMarkerProcessor(testItem, imagesDir);
        }

        private void ViewModel_TimesUpEvent(object sender, EventArgs args) => Dispatcher.Invoke(() =>
        {
            if(IsFinished)
            {
                return;
            }

            IsFinished = true;
            Close();
        });

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = ViewModel;

            GlobalValues.ExamWindow = this;

            Title = ViewModel.TestItem.Title;

            ViewModel.TopMost = false;

            ViewModel.NextQuestion();
        }

        private async void Window_Closed(object sender, EventArgs e)
        {
            ExamEndedEvent?.Invoke(ViewModel, null);

            await FinilizeExam();

            GlobalValues.ExamWindow = null;

            GlobalValues.TestMarker = null;
        }

        private void QuestionContainer_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var q = (TestItemQuestionViewModel)((FrameworkElement)sender).DataContext;

            q.IsSelected = q.IsSelected == null ? true : !q.IsSelected;

            ViewModel.UnSelectedQuestions(q);

            if (ViewModel.SelectedQuestion == q)
            {
                ViewModel.SelectedQuestion = null;
            }
            else
            {
                ViewModel.SelectedQuestion = q;
                SetScrollForScrollViewer();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NextQuestion();
            SetScrollForScrollViewer();
        }

        private void SetScrollForScrollViewer()
        {
            var element = (FrameworkElement)QuestionsItemsControl.ItemContainerGenerator.ContainerFromIndex(ViewModel.SelectedQuestion.Order - 1);
            element?.BringIntoView();
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.PrevQuestion();
            SetScrollForScrollViewer();
        }

        private void ChangeViewButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedQuestion == null)
            {
                ViewModel.NextQuestion();
            }

            Hide();

            var window = new ExamAltWindow
            {
                ParentViewModel = ViewModel
            };

            window.ShowDialog();

            SetScrollForScrollViewer();

            Show();
        }

        private async Task FinilizeExam()
        {
            FinishButton.IsEnabled = false;

            try
            {
                using (var db = new DataService())
                {
                    await db.SaveExam(EventHookConfig.ExamTools.Records, startDateTime);
                }
            }
            catch (Exception)
            {
                NotificationsHelper.Error("خطا در ثبت اطلاعات آزمون", "خطا");
            }

            GlobalValues.ExamIsRunning = false;
            GlobalValues.Question = null;
            GlobalValues.Student = null;
            GlobalValues.Test = null;

            FinishButton.IsEnabled = true;

            NotificationsHelper.Information("آزمون شما با موفیت ذخیره شد.", "تشکر");
        }

        private async void AnswerButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterAnswerButton.IsEnabled = false;

            var btnContent = RegisterAnswerButton.Content;
            var bg = RegisterAnswerButton.Background;
            var width = RegisterAnswerButton.ActualWidth;
            RegisterAnswerButton.Content = new MaterialDesignThemes.Wpf.PackIcon
            {
                Kind = MaterialDesignThemes.Wpf.PackIconKind.Tick
            };
            RegisterAnswerButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.YellowGreen);

            await ExamTools.TakeFinalRecord();

            RegisterAnswerButton.Width = width;
            RegisterAnswerButton.Content = btnContent;
            RegisterAnswerButton.Background = bg;

            RegisterAnswerButton.IsEnabled = true;
        }

        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(IsFinished)
            {
                return;
            }

            var q = NotificationsHelper.Ask("آیا از اعلام اتمام امتحان مطمئن هستید؟", "مهم");

            if (q != MessageBoxResult.OK)
            {
                e.Cancel = true;
            }
        }

        private void ShowHintButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ViewModel.SelectedQuestion?.Hint))
            {
                return;
            }

            NotificationsHelper.Information(ViewModel.SelectedQuestion.Hint, "راهنمایی");
        }
    }
}
