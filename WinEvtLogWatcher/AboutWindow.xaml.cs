using System.Reflection;
using System.Windows;

namespace WinEvtLogWatcher
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : DevExpress.Xpf.Core.ThemedWindow
    {
        public AboutWindow()
        {
            InitializeComponent();
            SetText();
        }

        private void SetText()
        {
            aboutTextBlock.Text += "ООО \"Транснефть-Восток\"\n";
            aboutTextBlock.Text += "Отдел автоматизированных систем управления технологическими процессами\n\n";
            aboutTextBlock.Text += "Собственность на данное программное обеспечение принадлежит ООО \"Транснефть-Восток\"\n";
            aboutTextBlock.Text += "Бесплатно для использования в организациях системы Транснефть\n\n";
            aboutTextBlock.Text += "Разработка: NikolaevIK@vsmn.transneft.ru (6881) 5863\n";
            aboutTextBlock.Text += $"Версия: {Assembly.GetExecutingAssembly().GetName().Version}";
        }

        private void restoreDefaultButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (MessageBox.Show("Восстановить до заодских настроек?", "Восстановление :)", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                Settings.GetInstance().RestoreDefault();
        }
    }
}
