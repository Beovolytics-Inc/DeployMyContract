using System;
using System.Globalization;
using System.Windows.Data;

namespace DeployMyContract.Wpf.Converters
{
    public class UnixTimestampConverter : IValueConverter
    {
        private static readonly DateTime M_EpochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string timestampStr))
                return DateTime.UtcNow;
            if (!long.TryParse(timestampStr, out var timestamp))
                return DateTime.UtcNow;

            return M_EpochStart.AddSeconds(timestamp);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DateTime dateTime))
                return Binding.DoNothing;
            return ((int)(dateTime - M_EpochStart).TotalSeconds).ToString();
        }
    }
}
