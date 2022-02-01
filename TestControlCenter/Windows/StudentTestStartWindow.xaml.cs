using System.Windows;
using TestControlCenterDomain;
using TestControlCenter.Models;
using TestControlCenter.Services;
using TestControlCenter.Tools;

namespace TestControlCenter.Windows
{
    /// <summary>
    /// Interaction logic for TestStartWindow.xaml
    /// </summary>
    public partial class StudentTestStartWindow : Window
    {
        public TestStartViewModel ViewModel { get; private set; }

        public bool StartExam { get; set; } = false;

        public StudentTestStartWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel = await ViewModelsHelper.GetTestStartViewModelForStudent();
            studentNameButtonText.Text = $"برای ({ViewModel.SelectedStudent.Name})";

            if (ViewModel.TestItem == null)
            {
                App.LogoutLogin(this);
                return;
            }

            DataContext = ViewModel;

            if(ViewModel.TestItem?.Questions == null || ViewModel.TestItem.Questions.Count == 0)
            {
                NotificationsHelper.Error("برای این آزمون هیچ سوالی ثبت نشده است.", "خطا");
                Close();
            }

            LoadingElem.Visibility = Visibility.Hidden;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MoveWindowPart_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if(e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void DataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(e.RemovedItems.Count>0)
            {
                ViewModel.SelectedStudent = null;
            }

            if(e.AddedItems.Count>0)
            {
                ViewModel.SelectedStudent = e.AddedItems[0] as Student;
            }
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if(ViewModel.SelectedStudent == null)
            {
                NotificationsHelper.Warning("لطفا آزمون دهنده را مشخص کنید.", "اخطار");
                return;
            }

            ViewModel.SecondPhase = true;
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = false;

            var result =  await CommunicationService.StartTest(ViewModel.TestItem, ViewModel.SelectedStudent);

            StartButton.IsEnabled = true;

            if (!result)
            {
                NotificationsHelper.Error("امکان شروع آزمون وجود ندارد.", "خطا");
                return;
            }

            StartExam = true;

            if (ViewModel.TestItem.Questions.Count < 1)
            {
                NotificationsHelper.Error("آزمون سوالی ندارد! امکان شروع آزمون نیست.", "خطا");
                return;
            }

            var window = new ExamWindow(ViewModel.TestItem, ViewModel.SelectedStudent)
            {
                Owner = this
            };

            Hide();

            window.Show();

            window.ExamEndedEvent += Window_ExamEndedEvent;
        }

        private void Window_ExamEndedEvent(object sender, System.EventArgs e)
        {
            Close();
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            App.LogoutLogin(this);
        }
    }
}
