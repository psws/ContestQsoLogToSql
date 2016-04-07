using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfModalDialog
{
    /// <summary>
    /// Interaction logic for ModalMsgDialog.xaml
    /// </summary>
    ///
    // add this line to namespace withbassembly=
    //        xmlns:dialog1="clr-namespace:WpfModalDialog;assembly=WpfModalDialog"

    //add this line to xaml grid
    //        <dialog1:ModalMsgDialog x:Name="ModalMsgDialog"  Margin="0,-2,-12,-30" Visibility="Hidden"/>

    public partial class ModalMsgDialog : UserControl
    {
        public ModalMsgDialog()
        {
            InitializeComponent();
            Visibility = Visibility.Hidden;
        }


        private bool _hideRequest = false;
        private bool _result = false;
        private UIElement _parent;

        public void SetParent(UIElement parent)
        {
            _parent = parent;
        }

        #region Message

        //add this Text="{Binding Message}" clause to the xaml textblock
        //     <TextBlock x:Name="DialogMessageTextBlock" Text="{Binding Message}" TextWrapping="Wrap" Margin="55,28,10,30" Grid.Column="1"/>

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.
        // This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(
                "Message", typeof(string), typeof(ModalMsgDialog), new UIPropertyMetadata(string.Empty));

        #endregion

        public bool ShowHandlerDialog(string message)
        {
            Message = message;
            Visibility = Visibility.Visible;

            _parent.IsEnabled = true;
            //this.IsEnabled = true;

            _hideRequest = false;
            while (!_hideRequest)
            {
                // HACK: Stop the thread if the application is about to close
                if (this.Dispatcher.HasShutdownStarted ||
                    this.Dispatcher.HasShutdownFinished)
                {
                    break;
                }

                // HACK: Simulate "DoEvents"
                this.Dispatcher.Invoke(
                    DispatcherPriority.Background,
                    new ThreadStart(delegate { }));
                Thread.Sleep(20);
            }

            return _result;
        }

        private void HideHandlerDialog()
        {
            _hideRequest = true;
            Visibility = Visibility.Hidden;
            _parent.IsEnabled = true;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            _result = true;
            HideHandlerDialog();
        }



    }
}
