using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using TestControlCenterDomain;
using TestControlCenter.Infrastructure;
using TestControlCenter.Tools;
using System.IO;
using System.Windows.Controls;

namespace TestControlCenter.Models
{
    public class TestItemQuestionViewModel : TestItemQuestion, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private bool? isSelected;
        public bool? IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        private bool isAnswered = false;
        public bool IsAnswered
        {
            get => isAnswered;
            set
            {
                isAnswered = value;
                OnPropertyChanged(nameof(IsAnswered));
                OnPropertyChanged(nameof(IsNotAnswered));
            }
        }

        public bool IsNotAnswered
        {
            get => !IsAnswered;
        }

        private bool bookmarked;
        public bool Bookmarked
        {
            get => bookmarked;
            set
            {
                bookmarked = value;
                OnPropertyChanged(nameof(Bookmarked));
                OnPropertyChanged(nameof(NotBookmarked));
            }
        }

        public bool NotBookmarked
        {
            get => !Bookmarked;
        }
    }

    public class ExamViewModel : INotifyPropertyChanged
    {
        public Rect WorkArea { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public delegate void TimesUpEventHandler(object sender, EventArgs args);
        public event TimesUpEventHandler TimesUpEvent;

        internal void UnSelectedQuestions(TestItemQuestionViewModel except)
        {
            foreach (var item in AllQuestions)
            {
                if(item == except)
                {
                    continue;
                }

                item.IsSelected = false;
            }

            if(!AllQuestions.Any(x => x.IsSelected == true))
            {
                foreach (var item in AllQuestions)
                {
                    item.IsSelected = null;
                }
            }
        }

        public bool IsThereHint
        {
            get
            {
                return !string.IsNullOrEmpty(SelectedQuestion?.Hint);
            }
        }

        public void NextQuestion()
        {
            if (SelectedQuestion == null)
            {
                SelectedQuestion = AllQuestions.First();
                SetSelectedOnChange();
                return;
            }

            var order = SelectedQuestion.Order;
            var next = order + 1;
            if (next == AllQuestions.Count() + 1)
            {
                next = 1;
            }

            var wanted = AllQuestions.FirstOrDefault(x => x.Order == next);
            if (wanted == null)
            {
                return;
            }

            SelectedQuestion = wanted;
            SetSelectedOnChange();
        }

        private void SetSelectedOnChange()
        {
            SelectedQuestion.IsSelected = true;
            UnSelectedQuestions(SelectedQuestion);
        }

        public void PrevQuestion()
        {
            if (SelectedQuestion == null)
            {
                SelectedQuestion = AllQuestions.Last();
                SetSelectedOnChange();
                return;
            }

            var order = SelectedQuestion.Order;
            var next = order - 1;
            if (next == 0)
            {
                next = AllQuestions.OrderBy(x => x.Order).Last().Order;
            }

            var wanted = AllQuestions.FirstOrDefault(x => x.Order == next);
            if (wanted == null)
            {
                return;
            }

            SelectedQuestion = wanted;
            SetSelectedOnChange();
        }

        public Timer Timer { get; }

        private int secondsCounter = 0;
        public ExamViewModel()
        {
            Timer = new Timer((obj) =>
            {
                if(TestItem == null)
                {
                    return;
                }
                secondsCounter++;
                var time = (TestItem.TimeAllowedInMin * 60) - secondsCounter;
                RemainingTime = (new TimeSpan(0, 0, time)).ToString(@"mm\:ss");

                if(time == 0)
                {
                    TimesUpEvent?.Invoke(this, null);
                }
            }, null, 0, 1000);
        }

        private int height;
        public int Height
        {
            get => height;
            set
            {
                height = value;
                OnPropertyChanged(nameof(Height));
            }
        }

        private int minWidth;
        public int MinWidth
        {
            get => minWidth;
            set
            {
                minWidth = value;
                OnPropertyChanged(nameof(MinWidth));
            }
        }

        private int minHeight;
        public int MinHeight
        {
            get => minHeight;
            set
            {
                minHeight = value;
                OnPropertyChanged(nameof(MinHeight));
            }
        }

        private int width;
        public int Width
        {
            get => width;
            set
            {
                width = value;
                OnPropertyChanged(nameof(Width));
            }
        }

        private double left;
        public double Left
        {
            get => left;
            set
            {
                left = value;
                OnPropertyChanged(nameof(Left));
            }
        }

        private double top;
        public double Top
        {
            get => top;
            set
            {
                top = value;
                OnPropertyChanged(nameof(Top));
            }
        }

        public void SetLocationToBottomRight()
        {
            Left = WorkArea.Right - Width;
            Top = WorkArea.Bottom - Height;
        }

        private bool topMost;
        public bool TopMost
        {
            get => topMost;
            set
            {
                topMost = value;
                OnPropertyChanged(nameof(TopMost));
            }
        }

        private TestItemQuestionViewModel selectedQuestion;
        public TestItemQuestionViewModel SelectedQuestion
        {
            get => selectedQuestion;
            set
            {
                selectedQuestion = value;
                OnPropertyChanged(nameof(SelectedQuestion));
                OnPropertyChanged(nameof(IsQuestionSelected));
                OnPropertyChanged(nameof(IsThereHint));
                GlobalValues.Question = selectedQuestion;
            }
        }

        public bool IsQuestionSelected => SelectedQuestion != null;

        private TestItem testItem;
        public TestItem TestItem
        {
            get => testItem;
            set
            {
                testItem = value;
                var mapper = new Mapper(Mappings.TestItemQuestionMapperConfiguration);
                AllQuestions = mapper.Map<IEnumerable<TestItemQuestionViewModel>>(TestItem.Questions.OrderBy(x => x.Order));
            }
        }

        public Student Student { get; set; }

        private string remainingTime;

        public string RemainingTime
        {
            get => remainingTime;
            set
            {
                remainingTime = value;
                OnPropertyChanged(nameof(RemainingTime));
            }
        }

        private string filter;
        public string Filter
        {
            get => filter;
            set
            {
                filter = value;
                OnPropertyChanged(nameof(Questions));
            }
        }

        public IEnumerable<TestItemQuestionViewModel> AllQuestions { get; set; }

        public IEnumerable<TestItemQuestionViewModel> Questions
        {
            get
            {
                if (string.IsNullOrEmpty(Filter))
                {
                    return AllQuestions;
                }

                return AllQuestions.Where(x => x.Question.ToLower().Contains(Filter.ToLower())).ToList();
            }
        }

        public string FilesHandler(object sender)
        {
            Button button = null;
            if(sender is Button btn)
            {
                button = btn;
                button.IsEnabled = false;
            }

            var filesDir = GlobalTools.GetFilesDir(TestItem);

            var newDir = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\TCC\\{TestItem.CourseId}\\{Student.Token}";
            if(!Directory.Exists(newDir))
            {
                Directory.CreateDirectory(newDir);
            }

            var files = Directory.GetFiles(filesDir, "*.*");
            foreach (var file in files)
            {
                if (!File.Exists(file))
                {
                    continue;
                }

                var path = $"{newDir}\\{Path.GetFileName(file)}";
                if (File.Exists(path))
                {
                    continue;
                }

                File.Copy(file, path);
            }

            if(button != null)
            {
                button.IsEnabled = true;
            }

            return newDir;
        }
    }
}
