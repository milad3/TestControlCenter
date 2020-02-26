using System.Threading.Tasks;
using System.Windows;
using TestControlCenter.Models;
using TestControlCenter.Services;
using TestControlCenterDomain;

namespace TestControlCenter.Windows
{
    /// <summary>
    /// Interaction logic for TestsListWindow.xaml
    /// </summary>
    public partial class TestsListWindow : Window
    {
        public TestsListViewModel ViewModel { get; set; }

        public TestsListWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = ViewModel;
        }

        private void SortOrderComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ViewModel.SortOrder = SortOrderComboBox.SelectedIndex == 0 ? TestsListSortOrder.Newest : TestsListSortOrder.Oldest;
        }

        private void DataGrid_LoadingRow(object sender, System.Windows.Controls.DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            await Search();
        }

        private async Task Search()
        {
            SearchTextBox.IsEnabled = false;
            SearchButton.IsEnabled = false;
            await ViewModel.Search();
            SearchButton.IsEnabled = true;
            SearchTextBox.IsEnabled = true;
        }

        private async void SearchTextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Return)
            {
                await Search();
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (TestsDataGrid.SelectedItem == null)
            {
                return;
            }

            var wanted = (TestMark)TestsDataGrid.SelectedItem;

            var window = new ExamDetailsWindow(wanted)
            {
                Owner = this
            };

            window.ShowDialog();
        }
    }
}
