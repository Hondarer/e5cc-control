using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace e5ccControlPanel.Converters
{
    public class StatusBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && boolValue == true)
            {
                return Brushes.Orange;
            }

            return new SolidColorBrush(Color.FromArgb(255,134,73,0));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
