using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using TestControlCenter.Tools;
using TestControlCenterDomain;

namespace TestControlCenter.Models
{
    public class TestMarkingViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        readonly string imagesDir;

        public TestMarkingViewModel(TestMark testMark)
        {
            TestMark = testMark;
            imagesDir = GlobalTools.GetImagesDir(testMark.TestItem);
        }

        ITestMarker testMarker;

        public TestMark TestMark { get; set; }

        public ObservableCollection<TestMarkAnswer> Answers { get; set; }

        private double loadingValue;
        public double LoadingValue { get => loadingValue; set { loadingValue = value; OnPropertyChanged(nameof(LoadingValue)); } }

        private int markingCounter;
        public async Task<bool> Mark(Dispatcher dispatcher)
        {
            LoadProcessor();

            if (testMarker == null)
            {
                return false;
            }

            LoadingValue = 0;
            markingCounter = 0;

            Answers.Clear();

            await Task.Run(() =>
            {
                foreach (var question in TestMark.TestItem.Questions.OrderBy(x => x.Order))
                {
                    var wanted = TestMark.TestMarkAnswers.FirstOrDefault(x => x.TestItemQuestionId == question.Id);
                    if (wanted == null)
                    {
                        dispatcher.Invoke(() =>
                        {
                            Answers.Add(new TestMarkAnswer
                            {
                                PrivateScore = 0,
                                PublicScore = 0,
                                TestItemQuestion = question,
                                TestItemQuestionId = question.Id
                            });

                            markingCounter++;
                            LoadingValue = GetLoadingValue();
                        });
                    }
                    else
                    {
                        var result = testMarker.Mark(wanted, 0);

                        dispatcher.Invoke(() =>
                        {
                            Answers.Add(result.TestMarkAnswer);

                            markingCounter++;
                            LoadingValue = GetLoadingValue();
                        });
                    }
                }
            });

            UnLoadProcessor();

            return true;
        }

        private void UnLoadProcessor()
        {
            if(testMarker == null)
            {
                return;
            }

            testMarker = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private void LoadProcessor()
        {
            testMarker = GlobalTools.LoadTestMarkerProcessor(TestMark.TestItem, imagesDir);
        }

        private double GetLoadingValue()
        {
            return markingCounter * 100.0 / TestMark.TestMarkAnswers.Count;
        }
    }
}
