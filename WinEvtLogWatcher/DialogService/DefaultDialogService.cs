using Microsoft.Win32;
using System;
using System.Windows;

namespace WinEvtLogWatcher
{
    class DefaultDialogService : IDialogService
    {
        public string FilePath { get; set; }

        public bool OpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;
                return true;
            }
            return false;
        }

        public bool SaveFileDialog()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                FilePath = saveFileDialog.FileName;
                return true;
            }
            return false;
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }
    }
}
