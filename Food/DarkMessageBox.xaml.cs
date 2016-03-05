using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Food
{
    /// <summary>
    /// Interaction logic for DarkMessageBox.xaml
    /// </summary>
    public partial class DarkMessageBox : Window
    {
        string thisOpt;
        App thisApp = Application.Current as App;

        public DarkMessageBox(string textMessage, string caption, string option = "OK")
        {
            InitializeComponent();

            Left = (SystemParameters.PrimaryScreenWidth - Width) / 2;
            Top = (SystemParameters.PrimaryScreenHeight - Height) / 2;

            MessageTextTB.Text = textMessage;
            Title = caption;

            Visibility = Visibility.Collapsed;
            thisOpt = option;
            if (option == "OK")
            {
                OKB.Visibility = Visibility.Hidden;
                OKNolB.Content = "OK";
                CancelB.Visibility = Visibility.Hidden;
            }
            else if(option == "YesNoCancel")
            {
                OKB.Visibility = Visibility.Visible;
                OKNolB.Content = "НЕТ";
                CancelB.Visibility = Visibility.Visible;
            }
        }

        #region Template Window
        private void border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }
        private void closeButt_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void minButt_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region Шаблон окна
            //назначение обрабочтиков елементов шаблона
            object tmp = Template.FindName("topBorder", this);
            if (tmp != null) (tmp as Border).MouseLeftButtonDown += border_MouseLeftButtonDown;

            tmp = Template.FindName("closeButt", this);
            if (tmp != null) (tmp as Button).Click += closeButt_Click;

            tmp = Template.FindName("minButt", this);
            if (tmp != null) (tmp as Button).Click += minButt_Click;
            #endregion
        }

        private void OKB_Click(object sender, RoutedEventArgs e)
        {
            thisApp.lastDialogResult = "YES";
            Close();
        }

        private void OKNolB_Click(object sender, RoutedEventArgs e)
        {
            if(thisOpt == "OK")
                thisApp.lastDialogResult = "OK";
            else
            {
                thisApp.lastDialogResult = "NO";
            }
            Close();
        }

        private void CancelB_Click(object sender, RoutedEventArgs e)
        {
            thisApp.lastDialogResult = "CANCEL";
            Close();
        }
    }
}
