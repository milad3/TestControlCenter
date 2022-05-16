using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TestControlCenter.Models;

namespace TestControlCenter.Windows
{
    /// <summary>
    /// Interaction logic for ExamSummeryWindow.xaml
    /// </summary>
    public partial class ExamSummeryWindow : Window
    {
        private readonly ExamViewModel exam;

        public bool IsFinished { get; set; } = false;

        public TestItemQuestionViewModel SelectedTestItemQuestion { get; set; }

        public ExamSummeryWindow(ExamViewModel exam)
        {
            InitializeComponent();
            this.exam = exam;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            allQuestionsCount.Text = exam.AllQuestions.Count().ToString();

            answeredQuestionsCount.Text = exam.Questions.Count(x => x.IsAnswered).ToString();

            bookmarkedQuestionsCount.Text = exam.Questions.Count(x => x.Bookmarked).ToString();

            titleLabel.Content = exam.TestItem.Title;

            answeredQuestions.ItemsSource = exam.Questions.Where(x => x.IsAnswered);
            notAnsweredQuestions.ItemsSource = exam.Questions.Where(x => !x.IsAnswered);
            markedQuestions.ItemsSource = exam.Questions.Where(x => x.Bookmarked);
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            IsFinished = true;
            Close();
        }

        private void NotConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            IsFinished = false;
            Close();
        }

        private void Questions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(sender is ListView list)
            {
                if(list.SelectedItem != null)
                {
                    SelectedTestItemQuestion = (TestItemQuestionViewModel)list.SelectedItem;

                    IsFinished = false;
                    Close();
                }
            }
        }
    }
}
