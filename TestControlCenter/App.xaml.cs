using System.Windows;
using TestControlCenter.Infrastructure;
using TestControlCenter.Services;

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

            Mappings.AutoMapperConfig();

            EventHookConfig.Hook();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            EventHookConfig.Unhook();
        }
    }
}
