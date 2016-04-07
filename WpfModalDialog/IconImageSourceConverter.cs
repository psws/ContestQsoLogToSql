using System.Drawing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfModalDialog
{
    //http://stackoverflow.com/questions/2572734/how-do-i-use-standard-windows-warning-error-icons-in-my-wpf-app
    //To find this static resouce in DialogMsg you need to add this to <Window  section
    /*
      <Window x:Class="ContestQsoLogToSql.web.DialogMsg"
         xmlns:draw="clr-namespace:System.Drawing;assembly=System.Drawing"
         xmlns:local="clr-namespace:ContestQsoLogToSql.web"
    */
    //Yopu also need to add this
        /*
         *     <Window.Resources>
            <local:IconToImageSourceConverter x:Key="IconToImageSourceConverter" />
        </Window.Resources>
        */
    //You can then use this source in DialogMsg.xaml
    /*
       <Image x:Name="Image1" HorizontalAlignment="Left" Height="100" Margin="21,37,0,0" VerticalAlignment="Top" Width="100"  
               Source="{Binding Source={x:Static draw:SystemIcons.Warning},
        Converter={StaticResource IconToImageSourceConverter},
        Mode=OneWay}">
        </Image>

     */
    public class IconToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            var icon = value as Icon;
            if (icon == null)
            {
                Trace.TraceWarning("Attempted to convert {0} instead of Icon object in IconToImageSourceConverter", value);
                return null;
            }

            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            return imageSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
