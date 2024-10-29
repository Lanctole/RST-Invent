using System.Windows;

namespace RST_Invent.Services
{
    internal class NotificationService
    {
        public void ShowNotification(string message)
        {
            MessageBox.Show(message, "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}