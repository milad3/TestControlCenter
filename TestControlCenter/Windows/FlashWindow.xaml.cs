using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace TestControlCenter.Windows
{
    /// <summary>
    /// Interaction logic for FlashWindow.xaml
    /// </summary>
    public partial class FlashWindow : Window
    {
        public FlashWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var animation = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 500)));

            BeginAnimation(OpacityProperty, animation);

            await Task.Delay(500);

            Close();
        }
    }
}
