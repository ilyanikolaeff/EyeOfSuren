using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WinEvtLogWatcher.UserControls
{
    /// <summary>
    /// Interaction logic for SettingsUserControl.xaml
    /// </summary>
    public partial class SettingsUserControl : UserControl
    {
        public SettingsUserControl()
        {
            InitializeComponent();
            DataContext = new ViewModels.SettingsViewModel();
        }
        private void MaxDisplayEventsTextEdit_Validate(object sender, DevExpress.Xpf.Editors.ValidationEventArgs e)
        {
            if (e.Value is null) return;
            int.TryParse(e.Value.ToString(), out int maxEventsToDisplay);
            if (maxEventsToDisplay < 0 || maxEventsToDisplay > 10000)
            {
                e.IsValid = false;
                e.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Critical;
                e.ErrorContent = "Значение должно быть числом в диапазоне от 0 до 10000";
            }
        }

        private void DisplayEventsPeriodTextEdit_Validate(object sender, DevExpress.Xpf.Editors.ValidationEventArgs e)
        {
            if (e.Value is null) return;
            int.TryParse(e.Value.ToString(), out int maxEventsToDisplay);
            if (maxEventsToDisplay < 0 || maxEventsToDisplay > 60)
            {
                e.IsValid = false;
                e.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Critical;
                e.ErrorContent = "Значение должно быть числом в диапазоне от 0 до 60";
            }
        }
    }
}
