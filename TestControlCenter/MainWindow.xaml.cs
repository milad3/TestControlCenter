using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using TestControlCenterDomain;
using TestControlCenter.Models;
using TestControlCenter.Properties;
using TestControlCenter.Services;
using TestControlCenter.Tools;
using TestControlCenter.Windows;
using Microsoft.Win32;
using System.IO;

namespace TestControlCenter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Timer ConnectionTestTimer { get; }

        public Timer UnmarkedTestsCheckTimer { get; }

        public Timer GarbageCollectorTimer { get;  }

        public MainWindowViewModel ViewModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            ConnectionTestTimer = new Timer(GetConnectionCheckTimerCallback(), null, 0, 20 * 1000);

            UnmarkedTestsCheckTimer = new Timer(GetUnmarkedTestsCheckTimerCallback(), null, 0, 15 * 1000);

            GarbageCollectorTimer = new Timer((obj) =>
            {
                AppCleaner.CleanUp();
            }, null, 0, 30 * 1000);

            var loginWindow = new LoginWindow();

            loginWindow.ShowDialog();

            if (!loginWindow.IsLoggedIn)
            {
                Application.Current.Shutdown();
            }
        }

        private TimerCallback GetUnmarkedTestsCheckTimerCallback()
        {
            return async (obj) =>
            {
                using (var db = new DataService())
                {
                    if(await db.IsThereUnmarkedTests())
                    {
                        Dispatcher.Invoke(() =>
                        {
                            UnmarkedTestsIcon.Visibility = Visibility.Visible;
                            UnmarkedButton.Visibility = Visibility.Visible;
                        });
                    }
                    else
                    {
                        Dispatcher.Invoke(() =>
                        {
                            UnmarkedTestsIcon.Visibility = Visibility.Collapsed;
                            UnmarkedButton.Visibility = Visibility.Collapsed;
                        });
                    }
                }
            };
        }

        private int communicationCounter = 0;
        private void ViewModel_InternetCommunicationFinishedEvent(object sender, EventArgs e)
        {
            communicationCounter--;
            if (communicationCounter == 0)
            {
                CommunicationIcon.Visibility = Visibility.Collapsed;
            }
        }

        private void ViewModel_InternetCommunicationStartEvent(object sender, EventArgs e)
        {
            communicationCounter++;
            CommunicationIcon.Visibility = Visibility.Visible;
        }

        TimerCallback GetConnectionCheckTimerCallback()
        {
            void setDisconnect()
            {
                Dispatcher.Invoke(() =>
                {
                    DisconnectIcon.Visibility = Visibility.Visible;
                    ConnectIcon.Visibility = Visibility.Collapsed;
                });
            };
            return async (obj) =>
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        var result = await client.GetAsync(Settings.Default.ConnectionTestUrl);

                        if (result.IsSuccessStatusCode)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                ConnectIcon.Visibility = Visibility.Visible;
                                DisconnectIcon.Visibility = Visibility.Collapsed;
                            });
                        }
                        else
                        {
                            setDisconnect();
                        }
                    }
                }
                catch (Exception)
                {
                    setDisconnect();
                }
            };
        }

        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuToggleButton_Click(object sender, RoutedEventArgs e)
        {
            MainMenuPopup.IsOpen = !MainMenuPopup.IsOpen;
        }

        private void MainMenuPopup_Closed(object sender, EventArgs e)
        {

        }

        private void NotificationCloseButton_Click(object sender, RoutedEventArgs e)
        {
            NotificationHolder.Visibility = Visibility.Collapsed;
            NotificationTextBlock.Text = string.Empty;
        }

        private void SimpleMessageNotify(string message)
        {
            NotificationHolder.Visibility = Visibility.Visible;
            NotificationTextBlock.Text = message;
        }

        private async void GetNewTestItemsButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            btn.IsEnabled = false;

            await ViewModel.GetLatestTestItems();

            btn.IsEnabled = true;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ViewModel?.SetColumnsCount(MainContainer.RenderSize);
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            var elem = (Border)sender;
            var testItem = (TestItem)elem.DataContext;

            if (request != null && !request.IsCompleted && requestTestItem.Id != testItem.Id)
            {
                elem.Opacity = .5;
                elem.Cursor = Cursors.Arrow;
                return;
            }

            elem.Opacity = 1;
            elem.Cursor = Cursors.Hand;
            elem.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            var grid = (Grid)elem.Child;
            var hoverGrid = grid.Children[0];
            hoverGrid.Visibility = Visibility.Visible;
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            var elem = (Border)sender;
            elem.Opacity = 1;
            elem.Cursor = Cursors.Hand;
            elem.BorderBrush = new SolidColorBrush(Color.FromRgb(80, 80, 80));
            var grid = (Grid)elem.Child;
            var hoverGrid = grid.Children[0];
            hoverGrid.Visibility = Visibility.Hidden;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel = await ViewModelsHelper.GetMainWindowViewModel();

            ViewModel.InternetCommunicationStartEvent += ViewModel_InternetCommunicationStartEvent;
            ViewModel.InternetCommunicationFinishedEvent += ViewModel_InternetCommunicationFinishedEvent;
            DataContext = ViewModel;

            ViewModel.SetColumnsCount(MainContainer.RenderSize);
        }

        private void FilterTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (sender is TextBox textBox)
                {
                    textBox.IsEnabled = false;
                    ViewModel.Serach(textBox.Text);
                    textBox.IsEnabled = true;
                }
            }
        }

        Task<List<MftStudent>> request;
        TestItem requestTestItem;
        private async void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            var border = (Border)sender;
            
            var testItem = (TestItemViewModel)(border).DataContext;

            if (request != null && !request.IsCompleted && requestTestItem.Id != testItem.Id)
            {
                return;
            }

            requestTestItem = testItem;

            if (request != null && !request.IsCompleted)
            {
                NotificationsHelper.Information($"در حال دریافت اطلاعات آزمون دهندگان از سرورر؛ لطفا منتظر باشید.", "توجه");
                return;
            }
            testItem.IsLoadingStudents = true;

            request = ViewModel.GetStudents(testItem);
            var studens = await request;
            studens = studens.Select(x =>
            {
                if (string.IsNullOrEmpty(x.Mobile) || x.Mobile.Length < 10)
                {
                    return x;
                }

                return new MftStudent
                {
                    IdInServer = x.IdInServer,
                    Name = x.Name,
                    NationalCode = x.NationalCode,
                    Mobile = x.Mobile.Remove(4, 3).Insert(4, "***")
                };
            }).ToList();

            testItem.IsLoadingStudents = false;
            BlurTheBackground();

            var window = new TestStartWindow
            {
                Owner = this,
                TestItem = testItem,
                Students = studens
            };

            window.ShowDialog();

            UnBlurTheBackground();

            if (!window.StartExam)
            {
                return;
            }

            Visibility = Visibility.Hidden;

            StartExam(window.ViewModel.SelectedStudent, window.TestItem);
        }

        private void StartExam(MftStudent selectedStudent, TestItem testItem)
        {
            if(testItem.Questions.Count < 1)
            {
                NotificationsHelper.Error("آزمون سوالی ندارد! امکان شروع آزمون نیست.", "خطا");
                return;
            }

            var window = new ExamWindow(testItem, selectedStudent)
            {
                Owner = this
            };

            window.ExamEndedEvent += Window_ExamEndedEvent;

            window.Show();
        }

        private void Window_ExamEndedEvent(object sender, EventArgs e)
        {
            Visibility = Visibility.Visible;
        }

        private void UnBlurTheBackground()
        {
            MainContainerGrid.Effect = null;
            MainContainerGridBackDrop.Visibility = Visibility.Collapsed;
        }

        private void BlurTheBackground()
        {
            MainContainerGrid.Effect = new BlurEffect
            {
                Radius = 10
            };
            MainContainerGridBackDrop.Visibility = Visibility.Visible;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void LoadTestButton_Click(object sender, RoutedEventArgs e)
        {
            var fileSelectorDialog = new OpenFileDialog
            {
                DefaultExt = ".emft",
                Filter = "MFT Test File|*.emft"
            };

            var result = fileSelectorDialog.ShowDialog();

            if(result.Value != true)
            {
                return;
            }

            foreach (var fileName in fileSelectorDialog.FileNames)
            {
                if(!File.Exists(fileName))
                {
                    continue;
                }

                try
                {
                    var importer = new ImportExportHelper();
                    var testItem = await importer.Import(fileName);
                    NotificationsHelper.Information($"{testItem.Title}' اضافه / به روز رسانی شد.", "انجام شد");
                }
                catch (Exception ex)
                {
                    NotificationsHelper.Error($"خطا در ثبت '{Path.GetFileName(fileName)}'", $"خطا {ex.HResult}");
                }
            }

            await ViewModel.LoadTestItems();
        }

        private async void Export_Click(object sender, RoutedEventArgs e)
        {
            var item = (TestItem)((FrameworkElement)sender).DataContext;

            var saveFileDialog = new SaveFileDialog
            {
                FileName = $"{item.Title}.emft",
                OverwritePrompt = true,
                Filter = "MFT Test File|*.emft",
                DefaultExt = ".emft"
            };

            var result = saveFileDialog.ShowDialog();

            if(result.Value != true)
            {
                return;
            }

            var exporter = new ImportExportHelper();

            try
            {
                await exporter.Export(item, saveFileDialog.FileName);
                NotificationsHelper.Information("با موفقیت ذخیره شد.", "ذخیره فایل خروجی");
            }
            catch (Exception)
            {
                NotificationsHelper.Error("خطا در ذخیره سازی", "ذخیره فایل خروجی");
            }
        }

        private async void PinToggle_Click(object sender, RoutedEventArgs e)
        {
            var item = (TestItem)((FrameworkElement)sender).DataContext;

            using (var db = new DataService())
            {
                await db.TogglePinForTestItemAndSave(item);
            }

            await ViewModel.LoadTestItems();
        }

        private async void UnmarkedTestsButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new TestsListWindow
            {
                Owner = this,
                ViewModel = await ViewModelsHelper.GetTestsListViewModel(false, false, true, TestsListSortOrder.Newest)
            };
            
            window.ShowDialog();
        }

        private async void TestsButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new TestsListWindow
            {
                Owner = this,
                ViewModel = await ViewModelsHelper.GetTestsListViewModel(true, true, true, TestsListSortOrder.Newest)
            };

            window.ShowDialog();
        }
    }
}
