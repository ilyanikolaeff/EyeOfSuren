using System.Windows;

namespace WinEvtLogWatcher
{
    /// <summary>
    /// Interaction logic for AddComputerDialog.xaml
    /// </summary>
    public partial class AddComputerDialog
    {
        public AddComputerDialog()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        public string IpAddress
        {
            get
            {
                return $"{octet1.Text}.{octet2.Text}.{octet3.Text}.{octet4.Text}";
            }
        }

        private void octet_Validate(object sender, DevExpress.Xpf.Editors.ValidationEventArgs e)
        {
            if (e.Value is null) return;
            int.TryParse(e.Value.ToString(), out int intValue);
            if (intValue < 0 || intValue > 255)
            {
                e.IsValid = false;
                e.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Critical;
                e.ErrorContent = "Значение должно быть числом в диапазоне от 0 до 255";
            }
        }
    }
}
