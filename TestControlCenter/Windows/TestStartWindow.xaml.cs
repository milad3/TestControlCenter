using System.Collections.Generic;
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
    public partial class TestStartWindow : Window
    {
        public TestItem TestItem { get; set; }

        public List<MftStudent> Students { get; internal set; }
        
        public TestStartViewModel ViewModel { get; private set; }

        public bool StartExam { get; set; } = false;

        public TestStartWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel = ViewModelsHelper.GetTestStartViewModel(Students, TestItem);

            DataContext = ViewModel;

            if(ViewModel.TestItem?.Questions == null || ViewModel.TestItem.Questions.Count == 0)
            {
                NotificationsHelper.Error("برای این آزمون هیچ سوالی ثبت نشده است.", "خطا");
                Close();
            }
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
                ViewModel.SelectedStudent = e.AddedItems[0] as MftStudent;
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

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartExam = true;
            Close();
        }
    }
}
