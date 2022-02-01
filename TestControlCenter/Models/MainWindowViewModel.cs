using AutoMapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using TestControlCenterDomain;
using TestControlCenter.Infrastructure;
using TestControlCenter.Services;
using TestControlCenter.Tools;

namespace TestControlCenter.Models
{
    public enum TestsSortOrder
    {
        [Description("تاریخ ثبت نزولی")]
        NewsetAdded,
        [Description("تاریخ ثبت صعودی")]
        OldestAdded,
        [Description("تاریخ نزولی")]
        Newest,
        [Description("تاریخ صعودی")]
        Oldest,
        [Description("سطح")]
        Level,
        [Description("تعداد آزمون های انجام شده")]
        TestsDoneCounter
    }

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
            LoadinDataStartEventEvent += MainWindowViewModel_LoadinDataStartEventEvent;
        }

        private async void MainWindowViewModel_LoadinDataStartEventEvent(object sender, EventArgs e)
        {
            await UiUpdate();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public delegate void LoadinDataStartEventHandler(object sender, EventArgs e);
        public event LoadinDataStartEventHandler LoadinDataStartEventEvent;

        public delegate void InternetCommunicationStartEventHandler(object sender, EventArgs e);
        public event InternetCommunicationStartEventHandler InternetCommunicationStartEvent;

        public delegate void InternetCommunicationFinishedEventHandler(object sender, EventArgs e);
        public event InternetCommunicationFinishedEventHandler InternetCommunicationFinishedEvent;

        public delegate void InternetCommunicationErrorEventHandler(object sender, EventArgs e);
        public event InternetCommunicationErrorEventHandler InternetCommunicationErrorEvent;

        public string Username { get; set; }

        public string FriendlyUsername { get; set; }

        private int columns;

        public int Columns
        {
            get => columns;
            set
            {
                columns = value;
                OnPropertyChanged(nameof(Columns));
            }
        }

        private TestsSortOrder sortOrder;
        public TestsSortOrder SortOrder
        {
            get => sortOrder;
            set
            {
                sortOrder = value;
                LoadinDataStartEventEvent(null, null);
            }
        }

        private async Task UiUpdate()
        {
            LoadingDataCounter++;
            using (var db = new DataService())
            {
                UpdateTestsCollection(await db.GetTestsAsync(filter, sortOrder));
            }
            LoadingDataCounter--;
        }

        private string filter;
        public string Filter
        {
            get => filter;
            set
            {
                filter = value;
                LoadinDataStartEventEvent(null, null);
            }
        }

        public bool LoadingData
        {
            get
            {
                return loadingDataCounter > 0;
            }
        }

        private int loadingDataCounter;

        public int LoadingDataCounter
        {
            get => loadingDataCounter;
            set
            {
                loadingDataCounter = value;
                OnPropertyChanged(nameof(LoadingData));
            }
        }

        public async Task LoadTestItems()
        {
            LoadingDataCounter++;
            using (var db = new DataService())
            {
                var data = await db.GetTestsAsync(Filter, SortOrder);
                UpdateTestsCollection(data);
            }
            LoadingDataCounter--;
        }

        public ObservableCollection<TestItemViewModel> Tests { get; set; }

        public async Task GetLatestTestItems()
        {
            await RunAndRaiseCommunicationEvents<Task>(this, null, async () =>
            {
                var task = CommunicationService.GetNewTestItems();
                await task;

                using (var db = new DataService())
                {
                    var data = await db.GetTestsAsync(Filter, SortOrder);
                    UpdateTestsCollection(data);
                }
                return task;
            });
        }

        private async Task<T> RunAndRaiseCommunicationEvents<T>(object sender, EventArgs args, Func<Task<T>> operation)
        {
            InternetCommunicationStartEvent?.Invoke(sender, args);

            try
            {
                var result = await operation();
                InternetCommunicationFinishedEvent?.Invoke(sender, args);
                return result;
            }
            catch (Exception)
            {
                InternetCommunicationErrorEvent?.Invoke(sender, args);
            }

            InternetCommunicationFinishedEvent?.Invoke(sender, args);

            return default;
        }

        private void UpdateTestsCollection(IEnumerable<TestItem> data)
        {
            Tests.Clear();
            var mapper = new Mapper(Mappings.TestItemMapperConfiguration);

            foreach (var item in data)
            {
                Tests.Add(mapper.Map<TestItemViewModel>(item));
            }

            // TODO we need a better solution maybe?!

            //var listToBeRemoved = new List<TestItem>();
            //foreach (var item in Tests)
            //{
            //    if (!data.Any(x => x.Id == item.Id))
            //    {
            //        listToBeRemoved.Add(item);
            //    }
            //}
            //foreach (var item in listToBeRemoved)
            //{
            //    Tests.Remove(item);
            //}

            //foreach (var item in data)
            //{
            //    if (!Tests.Any(x => x.Id == item.Id))
            //    {
            //        Tests.Add(item);
            //    }
            //}
        }

        public async Task GetTestItems()
        {
            await RunAndRaiseCommunicationEvents<Task>(this, null, async () =>
            {
                var task = CommunicationService.GetTestItems();
                await task;

                using (var db = new DataService())
                {
                    var data = await db.GetTestsAsync(Filter, SortOrder);
                    UpdateTestsCollection(data);
                }
                return task;
            });
        }

        public void SetColumnsCount(Size size)
        {
            Columns = (int)size.Width / 250;
        }

        public void Serach(string filter)
        {
            Filter = filter;
        }

        public async Task<List<Student>> GetStudents(TestItem testItem)
        {
            return await RunAndRaiseCommunicationEvents(this, null, async () =>
            {
                return await CommunicationService.GetStudents(testItem);
            });
        }

        public async Task SyncTestItems()
        {
            await RunAndRaiseCommunicationEvents(this, null, async () =>
            {
                using (var db = new DataService())
                {
                    var data = await db.GetUnSyncedTestMarksAsync();

                    var task = Task.Run(async () =>
                    {
                        foreach (var item in data)
                        {
                            await CommunicationService.SyncTest(item);

                            item.IsSynced = true;
                            item.SyncDateTime = DateTime.Now;

                            await db.UpdateExam(item);
                        }
                    });

                    await task;
                    return task;
                }
            });
        }
    }

    public class TestItemViewModel : TestItem, INotifyPropertyChanged
    {
        public object CoverImageUri
        {
            get
            {
                var path = CoverImageAddress;
                if (!File.Exists(path))
                {
                    return "/TestControlCenter;component/Resources/emptycover.png";
                }

                return new Uri(path);
            }
        }

        public string TestLevelText
        {
            get
            {
                var result = string.Empty;

                switch (TestLevel)
                {
                    case TestLevel.Level0:
                        result = "ابتدایی";
                        break;
                    case TestLevel.Level1:
                        result = "ساده";
                        break;
                    case TestLevel.Level2:
                        result = "معمولی";
                        break;
                    case TestLevel.Level3:
                        result = "پیشرفته";
                        break;
                    case TestLevel.Level4:
                        result = "سخت";
                        break;
                    default:
                        break;
                }

                return result;
            }
        }

        public string PersianDateTime
        {
            get
            {
                return GlobalTools.GetPersianDate(DateTime);
            }
        }

        private bool isLoadingStudents = false;
        public bool IsLoadingStudents
        {
            get => isLoadingStudents;
            set
            {
                isLoadingStudents = value;
                OnPropertyChanged(nameof(IsLoadingStudents));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
