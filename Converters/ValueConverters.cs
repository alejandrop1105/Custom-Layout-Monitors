using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using CustomLayoutMonitors.Models;
using CustomLayoutMonitors.Utils;

namespace CustomLayoutMonitors.Converters
{
    public class ProfileToVisualItemsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DisplayProfile profile)
            {
                return ProfileVisualizer.GetVisualItems(profile).ToList();
            }
            return new List<MonitorVisualItem>();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CountToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // For text placeholder visibility
            if (parameter?.ToString() == "text")
            {
                if (value is string str)
                {
                    return string.IsNullOrEmpty(str) ? Visibility.Visible : Visibility.Collapsed;
                }
                return Visibility.Visible;
            }

            // For empty state visibility
            if (value is int count)
            {
                return count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class OrientationToImageConverter : IValueConverter
    {
        private static BitmapImage? _landscapeImage;
        private static BitmapImage? _portraitImage;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MonitorOrientation orientation)
            {
                if (orientation == MonitorOrientation.Portrait)
                {
                    _portraitImage ??= new BitmapImage(new Uri("pack://application:,,,/Assets/monitor_portrait.png"));
                    return _portraitImage;
                }
            }
            
            _landscapeImage ??= new BitmapImage(new Uri("pack://application:,,,/Assets/monitor_landscape.png"));
            return _landscapeImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isVisible)
            {
                // Support "Invert" parameter to reverse the logic
                bool invert = parameter?.ToString()?.Equals("Invert", StringComparison.OrdinalIgnoreCase) == true;
                if (invert) isVisible = !isVisible;
                
                return isVisible ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                bool result = visibility == Visibility.Visible;
                bool invert = parameter?.ToString()?.Equals("Invert", StringComparison.OrdinalIgnoreCase) == true;
                return invert ? !result : result;
            }
            return false;
        }
    }
}
