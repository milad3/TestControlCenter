using System.Windows;
using TestControlCenter.Infrastructure;
using TestControlCenter.Properties;
using TestControlCenter.Services;
using TestControlCenter.Windows;

namespace TestControlCenter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DataService.CreateDatabaseIfNeeded();
            MessageLogger.CreateDatabaseIfNeeded();

            Mappings.AutoMapperConfig();

            if (!Settings.Default.DevelopmentMode)
            {
                EventHookConfig.Hook();
            }

            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;

            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            LogoutLogin(null);
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (MessageBox.Show(e.Exception.Message, "خطا", MessageBoxButton.YesNoCancel, MessageBoxImage.Error) == MessageBoxResult.No)
            {
                if (MessageBox.Show("برنامه بسته شود؟", "توجه", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Current.Shutdown();
                }
            }
            e.Handled = true;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            EventHookConfig.Unhook();
        }

        public static void LogoutLogin(Window sender)
        {
            if (sender != null)
            {
                sender.Hide();
            }

            ServerClient.AuthenticationData = null;
            StaticValues.LoggedInUserType = UserType.None;

            var loginWindow = new LoginWindow();

            loginWindow.ShowDialog();

            if (!loginWindow.IsLoggedIn || StaticValues.LoggedInUserType == UserType.None)
            {
                Current.Shutdown();
            }

            if (StaticValues.LoggedInUserType == UserType.Operator)
            {
                foreach (var w in Current.Windows)
                {
                    if(w is MainWindow mainWindow)
                    {
                        mainWindow.Show();
                        return;
                    }
                }

                var window = new MainWindow();
                window.Show();
            }
            else if (StaticValues.LoggedInUserType == UserType.Student)
            {
                var newTestWindow = new StudentTestStartWindow();
                newTestWindow.Show();
            }
        }
    }
}
