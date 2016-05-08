using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Interaction logic for Buy.xaml
    /// </summary>
    public partial class Buy : Window
    {
        App thisApp = Application.Current as App;
        List<ProductElement> selected;
        string oldCount;
        string oldNumber = "";
        bool show;
        Order order;

        public Buy(List<ProductElement> selected)
        {
            InitializeComponent();

            Left = (SystemParameters.PrimaryScreenWidth - Width) / 2;
            Top = (SystemParameters.PrimaryScreenHeight - Height) / 2;

            thisApp.EnableCheck(false);

            this.selected = selected;
            show = false;

            order = new Order();
        }
        public Buy(int id)
        {
            InitializeComponent();

            Left = (SystemParameters.PrimaryScreenWidth - Width) / 2;
            Top = (SystemParameters.PrimaryScreenHeight - Height) / 2;

            thisApp.EnableCheck(false);
            show = true;

            var orders = thisApp.client.GetOrders();

            order = orders.First(x => x.Id == id);
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

            //tmp = prodLV.ItemTemplate.FindName("isCheckCB", prodLV);
            //if (tmp != null) (tmp as CheckBox).Checked += isCheckCB_Checked;
            #endregion

            if (show)
            {
                formB.Visibility = Visibility.Collapsed;
                cancelB.Visibility = Visibility.Collapsed;
                okB.Visibility = Visibility.Visible;

                nameTB.IsReadOnly = cvv2TB.IsReadOnly = dayTB.IsReadOnly = monthTB.IsReadOnly = nProductsTB.IsReadOnly = 
                   priceTB.IsReadOnly = yearTB.IsReadOnly = numb1TB.IsReadOnly= numb2TB.IsReadOnly = numb3TB.IsReadOnly = 
                   numb4TB.IsReadOnly = true;

                UserInfo user = thisApp.client.GetUserInfoId(order.IdCustomer);

                nOrderL.Content = order.Id;
                nameL.Content = user.FullName;
                addressL.Content = user.Address;

                string cardNumb = order.CardNumber;

                numb1TB.Text = cardNumb.Substring(0, 4);
                numb2TB.Text = cardNumb.Substring(4, 4);
                numb3TB.Text = cardNumb.Substring(8, 4);
                numb4TB.Text = cardNumb.Substring(12, 4);

                string [] date = order.CardDate.Split('/');

                dayTB.Text = date[0];
                monthTB.Text = date[1];
                yearTB.Text = date[2];

                nameTB.Text = user.FullName;
                cvv2TB.Text = order.CardCVV2;
                priceTB.Text = order.Price.ToString();

                productsLB.DataContext = order.products;
            }
            else
            {
                productsLB.DataContext = productsLB.DataContext = selected; ;
                nOrderL.Content = "XXXX";
                nameL.Content = thisApp.user.FullName;
                addressL.Content = thisApp.user.Address;
                nameTB.Text = thisApp.user.FullName;
                cvv2TB.Text = "000";

                foreach (var product in selected)
                {
                    product.Visibility = true;
                    product.Count = 1;
                }
            }        

            nProductsTB.IsEnabled = false;
        }

        private void productsLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (productsLB.SelectedItem == null)
            {
                nProductsTB.IsEnabled = false;
            }
            else
            {
                nProductsTB.IsEnabled = true;
                nProductsTB.Text = (productsLB.SelectedItem as ProductElement).Count.ToString();
                oldCount = nProductsTB.Text;
            }           
        }

        private void nProductsTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(show)
                return;

            try
            {
                (productsLB.SelectedItem as ProductElement).Count = Convert.ToInt32((sender as TextBox).Text);
                oldCount = (sender as TextBox).Text;
            }
            catch
            {
                if (oldCount != null)
                {
                    (productsLB.SelectedItem as ProductElement).Count = Convert.ToInt32(oldCount);
                    nProductsTB.Text = oldCount;
                }   
            }
        }

        private void numbTB_KeyDown(object sender, KeyEventArgs e)
        {
            if (show)
                return;

            oldNumber = (sender as TextBox).Text;
        }

        private void numbTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(show)
                return;

            try
            {
                Convert.ToInt32((sender as TextBox).Text);
            }
            catch
            {
                (sender as TextBox).Text = oldNumber;
                oldNumber = "";
            }
        }

        private void formB_Click(object sender, RoutedEventArgs e)
        {
            if (show)
                return;

            var selectedProducts = new List<ProductElement>();

            foreach (var product in selected)
            {
                if (product.IsSelected)
                {
                    selectedProducts.Add(product);
                }
            }

            var order = new Order()
            {
                products = selectedProducts.ToArray(),
                IdCustomer = thisApp.user.Id,
                Time = DateTime.Now,
                CardNumber = numb1TB.Text+numb2TB.Text+numb3TB.Text+numb4TB.Text,
                CardDate = dayTB.Text + "/" + monthTB.Text + "/" + yearTB.Text,
                CardCVV2 = cvv2TB.Text,
                //Price = 
            };

            Close();
        }

        private void cancelB_Click(object sender, RoutedEventArgs e)
        {
            if(show)
                return;

            Close();
        }

        private void productsLB_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (productsLB.SelectedItem == null || !(productsLB.SelectedItem is ProductElement))
                return;

            Product edProd = new Product((productsLB.SelectedItem as ProductElement).Id, false);
            edProd.ShowDialog();
        }
    }
}
