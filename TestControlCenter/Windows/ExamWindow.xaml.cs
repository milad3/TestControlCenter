using System;
using System.Windows;
using TestControlCenterDomain;
using TestControlCenter.Models;
using TestControlCenter.Services;
using TestControlCenter.Infrastructure;
using TestControlCenter.Tools;
using System.Threading.Tasks;
using System.Diagnostics;

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

        public delegate void ExamStartedEventHandler(object sender, EventArgs e);
        public event ExamStartedEventHandler ExamStartedEvent;

        public ExamViewModel ViewModel { get; set; }
        public bool IsFinished { get; private set; }

        public ExamWindow(TestItem testItem, Student student)
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

            var testBasicInformation = new TestBasicInformation
            {
                ImagesDirectory = GlobalTools.GetImagesDir(testItem),
                FilesDirectory  = GlobalTools.GetFilesDir(testItem),
                TestStartDateTime = DateTime.Now
            };

            GlobalValues.TestMarker = GlobalTools.LoadTestMarkerProcessor(testItem, testBasicInformation);
        }

        private void ViewModel_TimesUpEvent(object sender, EventArgs args) => Dispatcher.Invoke(() =>
        {
            if (IsFinished)
            {
                return;
            }

            IsFinished = true;
            Close();
        });

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = ViewModel;

            ExamStartedEvent?.Invoke(ViewModel, null);

            GlobalValues.ExamWindow = this;

            Title = ViewModel.TestItem.Title;

            ViewModel.TopMost = false;

            ViewModel.NextQuestion();
        }

        private async void Window_Closed(object sender, EventArgs e)
        {
            await FinilizeExam();

            ExamEndedEvent?.Invoke(ViewModel, null);

            GlobalValues.ExamWindow = null;

            GlobalValues.TestMarker = null;
        }

        private void QuestionContainer_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var q = (TestItemQuestionViewModel)((FrameworkElement)sender).DataContext;

            q.IsSelected = q.IsSelected == null ? true : !q.IsSelected;
            SelectQuestion(q);
        }

        private void SelectQuestion(TestItemQuestionViewModel q)
        {
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

            if (GlobalValues.Test.TerminateAfterExam)
            {
                var processes = GlobalValues.Test.Processes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var process in processes)
                {
                    var wanted = Process.GetProcessesByName(process);
                    foreach (var p in wanted)
                    {
                        p.Kill();
                    }
                }
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
            if (IsFinished)
            {
                return;
            }

            var summeryWindow = new ExamSummeryWindow(ViewModel)
            {
                Owner = this
            };
            summeryWindow.ShowDialog();

            if(summeryWindow.SelectedTestItemQuestion != null)
            {
                summeryWindow.SelectedTestItemQuestion.IsSelected = true;

                SelectQuestion(summeryWindow.SelectedTestItemQuestion);
            }

            if (!summeryWindow.IsFinished)
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

        private void BookmarkToggle_Click(object sender, RoutedEventArgs e)
        {
            var q = (TestItemQuestionViewModel)((FrameworkElement)sender).DataContext;

            q.Bookmarked = !q.Bookmarked;
        }

        private void FilesButton_Click(object sender, RoutedEventArgs e)
        {
            var address = ViewModel.FilesHandler(sender);

            if(address == null)
            {
                NotificationsHelper.Information("فایلی برای نمایش وجود ندارد.", "توجه");
                return;
            }

            Process.Start(address);
        }
    }
}
