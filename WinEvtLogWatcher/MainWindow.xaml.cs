using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using System;
using System.Windows.Media;
using System.IO;
using System.Linq;
using System.Windows;

namespace WinEvtLogWatcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private string layoutFileName = AppDomain.CurrentDomain.BaseDirectory + "Layout.xml";
        public MainWindow()
        {
            InitializeComponent();
            MaxWidth = SystemParameters.PrimaryScreenWidth;
            MaxHeight = SystemParameters.PrimaryScreenWidth;
            DataContext = new MainWindowViewModel();
        }

        private void ThemedWindow_Closed(object sender, EventArgs e)
        {
            Settings.GetInstance().Save();
            SaveLayout();
        }

        private void ThemedWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //RestoreLayout();
            DXSplashScreen.Close();
        }


        private void SaveLayout()
        {
            eventsGridControl.SaveLayoutToXml(layoutFileName);
        }

        private void RestoreLayout()
        {
            if (File.Exists(layoutFileName))
                eventsGridControl.RestoreLayoutFromXml(layoutFileName);
        }

        private void ThemedWindow_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                ShowInTaskbar = false;
            }
            if (WindowState == WindowState.Normal || WindowState == WindowState.Maximized)
            {
                ShowInTaskbar = true;
            }
        }

        private void colorPickerValueChanged(object sender, EditValueChangedEventArgs e)
        {
        }
    }
}
