using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TestControlCenter.Infrastructure;
using TestControlCenter.Models;
using TestControlCenter.Tools;

namespace TestControlCenter.Windows
{
    /// <summary>
    /// Interaction logic for ExamAltWindow.xaml
    /// </summary>
    public partial class ExamAltWindow : Window
    {
        public ExamViewModel ParentViewModel { get; set; }

        public ExamAltWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Title = $"پاسخ به سوال شماره {ParentViewModel.SelectedQuestion.Order}";
            DataContext = ParentViewModel;
            GlobalValues.ExamAltWindow = this;
        }

        bool _shown;
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if(_shown)
            {
                return;
            }

            _shown = true;

            RearrangeView();
        }

        private void RearrangeView()
        {
            Top = ParentViewModel.WorkArea.Bottom - ActualHeight;
            Left = ParentViewModel.WorkArea.Right - ActualWidth;
        }

        private void QuestionContainer_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void ChangeViewButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            ParentViewModel.PrevQuestion();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            ParentViewModel.NextQuestion();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RearrangeView();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
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

        private void Window_Closed(object sender, EventArgs e)
        {
            GlobalValues.ExamAltWindow = null;
        }
    }
}
