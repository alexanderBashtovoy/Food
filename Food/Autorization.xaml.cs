using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Food.ServiceReference1;

namespace Food
{
    /// <summary>
    /// Логика взаимодействия для Autorization.xaml
    /// </summary>
    public partial class Autorization : Window
    {
        //DataSet data;
        List<CategoryWithProduct> products;
        //List<int> selectedItems;
        Service1Client _client;
        string nullCatName = "Без Категории";
        string rank = "Покупатель";
        App thisApp;
        bool _entered = false;
        //int idCust;
        public Autorization()
        {
            InitializeComponent();

            thisApp = Application.Current as App;
            thisApp.PrevWindow = App.Window.Shop;
            thisApp.NextWindow = App.Window.Exit;
        }

        #region Template Window
        private void border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ButtonState == MouseButtonState.Pressed)
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

            _client = thisApp.client;
            ReloadData();
            thisApp.products = products;

            thisApp.EnableCheck(false);

            categoryCB.DataContext = products;
            categoryCB.DisplayMemberPath = "Name";

            var nullCat = products.Find(x => x.Name == nullCatName);

            if (nullCat == null)
                categoryCB.SelectedItem = nullCatName;
            else
                categoryCB.SelectedItem = nullCat;

            checkCustomer.IsChecked = true;

            Switch(false);
        }
        private void Checked(object sender, RoutedEventArgs e)
        {
            RadioButton selButton = sender as RadioButton;
            switch (selButton.Content.ToString())
            {              
                case "Администратор":
                {
                    //dMB = new DarkMessageBox("Администратор", "ОК");
                    //dMB.ShowDialog();
                    //MessageBox.Show("Администратор");
                    regB.Visibility = Visibility.Collapsed;
                    break;
                }
                case "Менеджер":
                {
                    //dMB = new DarkMessageBox("Менеджер", "ОК");
                    //dMB.Show();
                    //MessageBox.Show("Менеджер");
                    regB.Visibility = Visibility.Collapsed;
                    break;
                }                    
                case "Покупатель":
                {
                    //dMB = new DarkMessageBox("Менеджер", "ОК");
                    //dMB.Show();
                    //MessageBox.Show("Покупатель");
                    regB.Visibility = Visibility.Visible;
                    break;
                }               
            }

            rank = selButton.Content.ToString();
        }
        private void categoryCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (categoryCB.SelectedItem == null)
                return;
            //if()
            //categoryCB.Text = (categoryCB.SelectedItem as CategoryWithProduct).Name;
            CheckSelected();

            var categoryWithProduct = categoryCB.SelectedItem as CategoryWithProduct;
            if (categoryWithProduct != null)
                prodLV.DataContext = categoryWithProduct.poductList;
        }
        void CheckSelected()
        {
            bool coin = false;
            foreach (CategoryWithProduct item in products)
            {
                if (categoryCB.Text == item.Name)
                {
                    coin = true;
                    break;
                }
            }

            if (!coin)
            {
                categoryCB.SelectedItem = "Без Категории";
                categoryCB.Text = "Без Категории";
            }
        }
        private void inoutB_Click(object sender, RoutedEventArgs e)
        {
            if (!_entered)
            {
                //DarkMessageBox 
                if (loginTB.Text == "")
                {
                    MessageBox.Show("Не введен логин", "Ошибка");
                    return;
                }
                if (passwTB.Password == "")
                {
                    MessageBox.Show("Не введен пароль", "Ошибка");
                    return;
                }

                thisApp.user = _client.GetUserInfo(loginTB.Text, passwTB.Password, rank);
                //ReloadData();

                if (thisApp.user.Message != "OK")
                {
                    DarkMessageBox dMB = new DarkMessageBox(thisApp.user.Message, "Ошибка");
                    dMB.ShowDialog();

                    loginTB.Text = "";
                    passwTB.Password = "";

                    return;
                }

                if (checkAdmin.IsChecked.Value)
                {
                    Adminka adm = new Adminka();
                    adm.Show();
                    Close();
                }

                Switch(true);
                _entered = true;
                //thisApp.EnableCheck(true);
                //SetVisibility(true);
            }
            else
            {
                Switch(false);
                thisApp.user = null;
                _entered = false;
                //thisApp.EnableCheck(false);
                //SetVisibility(false);
            }
        }
        private void Switch(bool entered)
        {
            if (entered)
            {
                //DataRow[] tmpRows = DataBase.SelectInTable(data.Tables["customerTable"],
                //string.Format("login = '{0}' AND password = '{1}'", loginTB.Text, passwTB.Text));
                //DataRow [] tmpRows = data.Tables [ "customerTable" ].Select( string.Format( "login = '{0}' AND password = '{1}'",
                //    loginTB.Text, passwTB.Text ) );
                fullNameTB.Visibility = Visibility.Visible;
                fullNameTB.Text = thisApp.user.FullName;
                //Изображение
                imageUser.Visibility = Visibility.Visible;
                var stream = new MemoryStream(thisApp.user.Image);
                var bmImage = new BitmapImage ();
                bmImage.BeginInit();
                bmImage.StreamSource = stream;
                bmImage.EndInit();
                imageUser.Source = bmImage;
                //fullNameL.Location = new Point(Size.Width / 2 - fullNameL.Size.Width / 2, fullNameL.Location.Y);
                //fullNameL.Visible = true;
                buyB.Visibility = Visibility.Visible;
                cabinetB.Visibility = Visibility.Visible;
                inoutB.Content = "Выход";

                //prodLV.CheckBoxes = true;

                //enterL.Visible = false;
                enterAsL.Visibility = Visibility.Hidden;
                //enterAsL.Visible = false;
                //loginL.Visible = false;
                loginL.Visibility = Visibility.Hidden;
                //passwL.Visible = false;
                passwL.Visibility = Visibility.Hidden;
                //enterAsCB.Visible = false;
                checkCustomer.Visibility = Visibility.Hidden;
                checkAdmin.Visibility = Visibility.Hidden;
                //loginTB.Visible = false;
                loginTB.Visibility = Visibility.Hidden;
                loginTB.Text = "";
                //passwTB.Visible = false;
                passwTB.Visibility = Visibility.Hidden;
                passwTB.Password = "";
                //enterB.Visible = false;
                //regB.Visible = false;
                regB.Visibility = Visibility.Collapsed;

                thisApp.EnableCheck(true);
            }
            else
            {
                //enterL.Visible = true;
                
                //enterAsL.Visible = true;
                enterAsL.Visibility = Visibility.Visible;
                //loginL.Visible = true;
                loginL.Visibility = Visibility.Visible;
                //passwL.Visible = true;
                passwL.Visibility = Visibility.Visible;
                //enterAsCB.Visible = true;
                checkCustomer.Visibility =  Visibility.Visible;
                checkAdmin.Visibility =     Visibility.Visible;
                //loginTB.Visible = true;
                loginTB.Visibility = Visibility.Visible;
                //passwTB.Visible = true;
                passwTB.Visibility = Visibility.Visible;
                //enterB.Visible = true;
                //regB.Visible = true;
                regB.Visibility = Visibility.Visible;

                //prodLV.CheckBoxes = false;

                //fullNameL.Text = "";
                fullNameTB.Text = "";
                //fullNameTB.Visible = false;
                //buyB.Visible = false;
                buyB.Visibility = Visibility.Collapsed;
                //cabinetB.Visible = false;
                cabinetB.Visibility = Visibility.Collapsed;
                //outLL.Visible = false;
                inoutB.Content = "Вход";
                imageUser.Visibility = Visibility.Hidden;

                thisApp.EnableCheck(false);
            }
        }
        private void regB_Click(object sender, RoutedEventArgs e)
        {
            Customer reg = new Customer();
            reg.ShowDialog();
        }
        void ReloadData()
        {
            products = new List<CategoryWithProduct>(_client.GetProducts());
            if(thisApp.user != null)
                thisApp.user = _client.GetUserInfo(thisApp.user.Login, thisApp.user.Password, thisApp.user.Rank);

            prodLV.Items.Clear();
            categoryCB.DataContext = products;
        }
        private void prodLV_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (prodLV.SelectedItem == null || !(prodLV.SelectedItem is ProductElement))
                return;
            
            Product edProd = new Product((prodLV.SelectedItem as ProductElement).Id, false);
            edProd.ShowDialog();
        }
        private void buyB_Click(object sender, RoutedEventArgs e)
        {
            var selectedProducts = new List<ProductElement>();

            foreach (var category in products)
            {
                if(category.poductList == null)
                    continue;

                foreach (var product in category.poductList)
                {
                    if (product.IsSelected)
                    {
                        selectedProducts.Add(product);
                    }
                }
            }

            Buy newBuy = new Buy(selectedProducts);
            newBuy.ShowDialog();
        }
        private void cabinetB_Click(object sender, RoutedEventArgs e)
        {
            Customer cust = new Customer(thisApp.user);
            cust.ShowDialog();
        }

        void SetVisibility(bool visible)
        {
            foreach (CategoryWithProduct category in products)
            {
                if(category.poductList != null)
                foreach (var product in category.poductList)
                {
                    product.Visibility = visible;
                }
            }
        }
        private void isCheckCB_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void prodLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckRB();
        }
       
        private void prodLV_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Thread thread = new Thread(CheckRB);
            thread.Start();
        }
        void CheckRB()
        {
            Thread.Sleep(100);

            bool select = false;
            foreach (CategoryWithProduct category in products)
            {
                if (category.poductList != null)
                    foreach (var product in category.poductList)
                    {
                        if (product.IsSelected)
                        {
                            select = product.IsSelected;
                        }
                    }
            }

            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
            (ThreadStart)delegate ()
            {
                buyB.IsEnabled = select;
            });

        }
    }
}
