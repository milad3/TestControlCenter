using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TestControlCenter.Services;
using TestControlCenter.Tools;
using TestControlCenterDomain;

namespace TestControlCenter.Models
{
    public class TestMarkingViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        readonly string imagesDir;
        private readonly IMessageLogger logger;

        public TestMarkingViewModel(TestMark testMark, IMessageLogger logger)
        {
            TestMark = testMark;
            this.logger = logger;
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
                    void Marking()
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
                            var result = testMarker.Mark(wanted, .8);

                            dispatcher.Invoke(() =>
                            {
                                Answers.Add(result.TestMarkAnswer);

                                markingCounter++;
                                LoadingValue = GetLoadingValue();
                            });
                        }
                    }

                    var handled = false;
                    while(!handled)
                    {
                        try
                        {
                            Marking();
                            handled = true;
                        }
                        catch (Exception ex)
                        {
                            logger.LogMessage(new LogMessage
                            {
                                DateTime = DateTime.Now,
                                Message = ex.Message,
                                Details = ex.StackTrace,
                                Ref = ex.InnerException.Message,
                                Type = LogMessageType.Error
                            });

                            var result = NotificationsHelper.Ask($"خطا در تصحیح سوال {question.Order}, آیا تلاش مجدد برای تصحیح سوال صورت گیرد؟", "خطا");
                            if(result == MessageBoxResult.OK)
                            {
                                handled = false;
                            }
                            else if(result == MessageBoxResult.Cancel)
                            {
                                handled = true;
                            }
                        }
                    }              
                }
            });

            UnLoadProcessor();

            return true;
        }

        public async Task SaveResults()
        {
            TestMark.IsMarked = true;

            using (var db = new DataService())
            {
                await db.UpdateExam(TestMark);
            }
        }

        public async Task FinilizeResults()
        {
            TestMark.IsFinal = true;
            TestMark.FinilizeDateTime = DateTime.Now;

            using (var db = new DataService())
            {
                await db.UpdateExam(TestMark);
            }
        }

        public async Task<bool> Sync()
        {
            try
            {
                var result = await CommunicationService.SyncTest(TestMark);
                if(result == false)
                {
                    return false;
                }

                TestMark.IsSynced = true;
                TestMark.SyncDateTime = DateTime.Now;
                using (var db = new DataService())
                {
                    await db.UpdateExam(TestMark);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
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
