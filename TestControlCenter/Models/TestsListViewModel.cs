using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using TestControlCenter.Services;
using TestControlCenterDomain;

namespace TestControlCenter.Models
{
    public enum TestsListSortOrder
    {
        Oldest,
        Newest
    }

    public class TestsListViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private bool showFinilized;
        public bool ShowFinilized { get => showFinilized; set { showFinilized = value; OnPropertyChanged(nameof(ShowFinilized)); } }

        private bool showUnmarked;
        public bool ShowUnmarked { get => showUnmarked; set { showUnmarked = value; OnPropertyChanged(nameof(ShowUnmarked)); } }

        private bool showMarked;

        public bool ShowMarked { get => showMarked; set { showMarked = value; OnPropertyChanged(nameof(ShowMarked)); } }

        public string Filter { get; set; }

        private TestsListSortOrder sortOrder;
        public TestsListSortOrder SortOrder { get => sortOrder; set { sortOrder = value; OnPropertyChanged(nameof(IsSortNewest)); OnPropertyChanged(nameof(IsSortOldest)); } }
        public bool IsSortNewest { get => SortOrder == TestsListSortOrder.Newest; }
        public bool IsSortOldest { get => SortOrder == TestsListSortOrder.Oldest; }

        public bool IsResultEmpty { get => Tests.Count == 0; }

        public List<TestMark> Tests { get; set; }

        public async Task Search()
        {
            using (var db = new DataService())
            {
                Tests = await db.SearchTestMarks(ShowFinilized, ShowUnmarked, ShowMarked, Filter, SortOrder);
                OnPropertyChanged(nameof(Tests));
                OnPropertyChanged(nameof(IsResultEmpty));
            }
        }
    }
}
