using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace WinEvtLogWatcher
{
    class BoolToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? new BitmapImage(new Uri(@"Images/SoundActive.png", UriKind.RelativeOrAbsolute)) 
                : new BitmapImage(new Uri(@"Images/SoundNoActive.png", UriKind.RelativeOrAbsolute));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
