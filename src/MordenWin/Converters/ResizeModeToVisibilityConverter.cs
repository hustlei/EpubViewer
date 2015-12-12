using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace Lei.UI.Converters
{
    public class ResizeModeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ResizeMode mode;
            if (value is ResizeMode)
            {
                mode = (ResizeMode)value;
            }
            else
            {
                mode = ResizeMode.CanResize;
            }
            string str = parameter as string;
            if (str == "MaxButton")
            {
                if (mode == ResizeMode.CanMinimize || mode == ResizeMode.NoResize)
                    return Visibility.Collapsed;
                else
                    return Visibility.Visible;
            }
            else if (str == "MinButton")
            {
                if (mode == ResizeMode.NoResize)
                    return Visibility.Collapsed;
                else
                    return Visibility.Visible;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }


    }

      
}
