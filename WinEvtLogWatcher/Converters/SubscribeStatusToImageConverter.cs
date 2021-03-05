using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace WinEvtLogWatcher
{
    class SubscribeStatusToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? new BitmapImage(new Uri(@"pack://application:,,,/DevExpress.Images.v19.2;component/Images/Arrows/Play_32x32.png", UriKind.RelativeOrAbsolute))
                                : new BitmapImage(new Uri(@"pack://application:,,,/DevExpress.Images.v19.2;component/Images/Arrows/Stop_32x32.png", UriKind.RelativeOrAbsolute));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
