using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Food.ServiceReference1;
using Microsoft.Win32;

namespace Food
{
    /// <summary>
    /// Interaction logic for Adminka.xaml
    /// </summary>
    public partial class Adminka : Window
    {
        App thisApp;
        Service1Client _client;

        int nPassw = 4;
        //MemoryStream imageStream;
        List<CategoryWithProduct> products;
        private string format;
        //private Uri imageUri;
        string nullCatName = "Без Категории";
        ProductElement lastSelected;
        //Image thisTmpImage;

        public Adminka()
        {
            InitializeComponent();

            Left = (SystemParameters.PrimaryScreenWidth - Width) / 2;
            Top = (SystemParameters.PrimaryScreenHeight - Height) / 2;

            thisApp = Application.Current as App;
            _client = thisApp.client;
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

            delCategoryB.Focusable = false;
            delProductB.Focusable = false;
            editB.Focusable = false;

            loginTB.Text = thisApp.user.Login;
            passwTB.Text = thisApp.user.Password;
            emailTB.Text = thisApp.user.Email;
            nameTB.Text = thisApp.user.FullName;
            addressTB.Text = thisApp.user.Address;

            maleRB.IsChecked = thisApp.user.Sex;
            femaleRB.IsChecked = !thisApp.user.Sex;

            if (thisApp.user.Image != null && thisApp.user.Image.Length > 0)
            {
                //var bi = new BitmapImage();
                //bi.BeginInit();
                ////ms = new MemoryStream(thisApp.user.Image);
                //bi.StreamSource = new MemoryStream(thisApp.user.Image);
                //bi.EndInit();
                avatarPB.Source = ImageConveter.ByteArrayToImage(thisApp.user.Image);
            }

            products = thisApp.products;

            categoryCB.DataContext = thisApp.products;
            categoryCB.DisplayMemberPath = "Name";

            var nullCat = products.Find(x => x.Name == nullCatName);

            //if (nullCat == null)
                //categoryCB.SelectedItem = nullCatName;
            //else
            categoryCB.SelectedItem = nullCat;

            thisApp.EnableCheck(false);
        }
        private void selectImageB_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog selectImageOFD = new OpenFileDialog();

            selectImageOFD.InitialDirectory = "c:\\";
            selectImageOFD.Filter = "BMP (*.bmp)|*.bmp|JPG (*.jpg)|*.jpg|PNG (*.png)|*.png|GIF (*.gif)|*.gif";
            selectImageOFD.FilterIndex = 2;
            selectImageOFD.RestoreDirectory = true;

            if (selectImageOFD.ShowDialog() == true)
            {
                Image thisTmpImage = new Image();
                thisTmpImage.Source = avatarPB.Source.Clone();

                try
                {
                    //Image thisTmpImage = avatarPB;
                    format = selectImageOFD.FileName.Split('.').Last();
                    BitmapImage tmp_image = new BitmapImage();

                    tmp_image.BeginInit();
                    tmp_image.UriSource = new Uri(selectImageOFD.FileName);
                    tmp_image.EndInit();
                    avatarPB.Source = tmp_image;
                }
                catch (Exception ex)
                {
                    var drkMB = new DarkMessageBox("Не удается прочитать файл зображения с диска. Original error: " + ex.Message, "Ошибка");
                    drkMB.ShowDialog();
                    avatarPB = thisTmpImage;
                }
            }
        }
        private void changeB_Click(object sender, RoutedEventArgs e)
        {
            DarkMessageBox dmb;
            if (loginTB.Text == "")
            {
                dmb = new DarkMessageBox("Не указан логин", "Ошибка");
                dmb.ShowDialog();
                return;
            }
            if (passwTB.Text == "")
            {
                dmb = new DarkMessageBox("Не указан пароль", "Ошибка");
                dmb.ShowDialog();
                return;
            }
            if (emailTB.Text == "")
            {
                dmb = new DarkMessageBox("Не указан email", "Ошибка");
                dmb.ShowDialog();
                return;
            }

            //Проверка корректности email----------------- 
            string pattern =
                @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$";

            if (!Regex.IsMatch(emailTB.Text, pattern, RegexOptions.IgnoreCase))
            {
                dmb = new DarkMessageBox("Не корректный Email", "Ошибка");
                dmb.ShowDialog();
                return;
            }
            //----------------------------------------  

            if (passwTB.Text.Length < nPassw)
            {
                dmb = new DarkMessageBox("Пароль слишком короткий. Укажите пароль не короче " + nPassw + " символов", "Ошибка");
                dmb.ShowDialog();
                
                return;
            }

            var tmpUser = thisApp.user;
            var oldLogin = tmpUser.Login;
            tmpUser.Login = loginTB.Text;
            tmpUser.Password = passwTB.Text;
            tmpUser.Email = emailTB.Text;
            tmpUser.FullName = nameTB.Text;
            tmpUser.Address = addressTB.Text;

            tmpUser.Sex = maleRB.IsChecked.Value;
            tmpUser.Image = null;

            ImageConveter.SendImage(ImageConveter.ImageToByteArray(avatarPB.Source));

            dmb = new DarkMessageBox(_client.SetUserInfo(oldLogin, tmpUser), "Инфо");
            dmb.ShowDialog();
        }
        private void categoryCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (categoryCB.SelectedItem == null)
            {
                delCategoryB.IsEnabled = false;
                return;
            }
                
            //if()
            //categoryCB.Text = (categoryCB.SelectedItem as CategoryWithProduct).Name;
            //CheckSelected();

            var categoryWithProduct = categoryCB.SelectedItem as CategoryWithProduct;

            delCategoryB.IsEnabled = true;

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

            if (coin) return;

            //categoryCB.SelectedItem = "Без Категории";
            //categoryCB.Text = "Без Категории";
        }
        private void addCategory_Click(object sender, RoutedEventArgs e)
        {
            AddCategory addCat = new AddCategory();
            addCat.ShowDialog();

            ReloadData();
        }
        private void delCategoryB_Click(object sender, RoutedEventArgs e)
        {
            if (categoryCB.SelectedItem == null)
                return;

            if ((categoryCB.SelectedItem as CategoryWithProduct).Name == "Без Категории")
            {
                DarkMessageBox dmb = new DarkMessageBox("Категорию \"Без Категории\" удалить нельзя. Удалить из неё товары?", "Внимание", "YesNoCancel");
                    dmb.ShowDialog();
            }
            else
            {
                DarkMessageBox dmb = new DarkMessageBox("Удалить товары в удаляемой категории?", "Внимание", "YesNoCancel");
                    dmb.ShowDialog();
            }         

            if (thisApp.lastDialogResult == "YES")
            {
                var tmp = categoryCB.SelectedItem as CategoryWithProduct;

                var tmpList = new List< ProductElement>();
                foreach (var pod in tmp.poductList)
                {
                    tmpList.Add(new ProductElement()
                    {
                        Id = pod.Id,
                        Name = pod.Name,
                    });
                }

                var tmpCategory = new CategoryWithProduct() {Id = tmp.Id, Name = tmp.Name, poductList = tmpList.ToArray()};

                _client.Delete(categoryCB.SelectedItem as CategoryWithProduct, true);
            }
            else if (thisApp.lastDialogResult == "NO")
            {
                _client.Delete(new CategoryWithProduct() {Name = (categoryCB.SelectedItem as CategoryWithProduct).Name,
                    Id = (categoryCB.SelectedItem as CategoryWithProduct).Id}, false);
            }

            ReloadData();
        }

        private void addProductB_Click(object sender, RoutedEventArgs e)
        {
            Product newProduct = new Product();
            newProduct.ShowDialog();

            ReloadData();
        }

        private void editB_Click(object sender, RoutedEventArgs e)
        {
            if (prodLV.SelectedItem != null)
            {
                Product edit = new Product((prodLV.SelectedItem as ProductElement).Id, true);
                edit.ShowDialog();

                thisApp.products = _client.GetProducts().ToList();

                ReloadData();
            }
            else
            {
                DarkMessageBox dmb = new DarkMessageBox("Товар Не выбран", "Ошибка");
                dmb.ShowDialog();
            }                
        }

        private void delProductB_Click(object sender, RoutedEventArgs e)
        {
            ProductElement tmp = new ProductElement() {Id = (prodLV.SelectedItem as ProductElement).Id};

            _client.Delete(new CategoryWithProduct()
            {
                poductList = new ProductElement[]
                {
                    tmp
                }
            }, true);

            ReloadData();
        }

        private void prodLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (prodLV.SelectedItem != null)
            {
                delProductB.IsEnabled = editB.IsEnabled = true;
            }
            else
            {
                delProductB.IsEnabled = editB.IsEnabled = false;
            }
        }

        private void prodLV_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (prodLV.SelectedItem != null)
            {
                Product tmpProduct = new Product((prodLV.SelectedItem as ProductElement).Id, false);
                tmpProduct.ShowDialog();
            }
        }

        private void cancelB_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void ReloadData()
        {
            thisApp.products = _client.GetProducts().ToList();

            var tmp = thisApp.products.Find(x => x.Id == (categoryCB.SelectedItem as CategoryWithProduct).Id);

            if (categoryCB.SelectedItem == null || tmp == null)
            {
                categoryCB.DataContext = thisApp.products;
                categoryCB.SelectedItem = thisApp.products.First(x => x.Name == nullCatName);
                prodLV.DataContext = (categoryCB.SelectedItem as CategoryWithProduct).poductList;
                return;
            }

            var idSelectedCategory = (categoryCB.SelectedItem as CategoryWithProduct).Id;
            categoryCB.DataContext = thisApp.products;
            categoryCB.SelectedItem = thisApp.products.First(p => p.Id == idSelectedCategory);

            prodLV.DataContext = null;
            prodLV.DataContext = (categoryCB.SelectedItem as CategoryWithProduct).poductList;
        }
    }
}
