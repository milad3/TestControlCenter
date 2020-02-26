using System;
using System.Windows;

namespace TestControlCenter.Tools
{
    public class NotificationsHelper
    {
        internal static MessageBoxResult Information(string message, string title)
        {
            return MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
        }

        internal static MessageBoxResult Warning(string message, string title)
        {
            return MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
        }

        internal static MessageBoxResult Error(string message, string title)
        {
            return MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
        }

        internal static MessageBoxResult Ask(string message, string title)
        {
            return MessageBox.Show(message, title, MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.Cancel, MessageBoxOptions.RtlReading);
        }
    }
}
