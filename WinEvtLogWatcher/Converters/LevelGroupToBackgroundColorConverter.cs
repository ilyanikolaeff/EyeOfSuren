using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace WinEvtLogWatcher
{
    class LevelGroupToBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
          
            var stdLevel = (StandardEventLevel)value;
            var color = Settings.GetInstance().DefinedColorSettings.FirstOrDefault(p => p.Name == stdLevel.ToString()).BackgroundColor;
            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
