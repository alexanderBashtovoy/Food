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
using Food.ServiceReference1;

namespace Food
{
    /// <summary>
    /// Interaction logic for AddCategory.xaml
    /// </summary>
    public partial class AddCategory : Window
    {
        App thisApp = Application.Current as App;
        public AddCategory()
        {
            InitializeComponent();
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

        private void addCategoryB_Click(object sender, RoutedEventArgs e)
        {
            if (categoryNameTB.Text == "")
            {
                DarkMessageBox dmb = new DarkMessageBox("Ничего не введено", "Ошибка");
                dmb.ShowDialog();
                return;
            }

            thisApp.client.Add(new CategoryWithProduct() { Name = categoryNameTB.Text });
            thisApp.products = thisApp.client.GetProducts().ToList();
            thisApp.products.Add(new CategoryWithProduct() {Name = categoryNameTB.Text});

            Close();
        }

        private void cancelB_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
