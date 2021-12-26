using System;
using System.Windows;
using System.Windows.Data;

namespace Norav.HRM.Client.WPF.Converters
{
    [Localizability(LocalizationCategory.NeverLocalize)]
    public sealed class InvertedBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => 
            value is true ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => 
            value is not Visibility.Visible;
    }
}