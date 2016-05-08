using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Win32;

namespace Food
{
    /// <summary>
    /// Interaction logic for AddProduct.xaml
    /// </summary>
    public partial class Product : Window
    {
        private Uri imageUri;
        private string format;
        string nullCatName = "Без Категории";
        App thisApp = Application.Current as App;
        ProductElement tmpProduct;
        bool add;
        bool edit;
        int id;
        Image thisTmpImage;

        public Product()
        {
            InitializeComponent();
            categoryCB.DataContext = thisApp.products;
            tmpProduct = new ProductElement();
            Title = "Добавление товара";
            add = true;
        }
        public Product(int id, bool edit)
        {
            InitializeComponent();
            categoryCB.DataContext = thisApp.products;

            foreach (var category in thisApp.products)
            {                
                tmpProduct = category.poductList.ToList().Find(x => x.Id == id);

                if (tmpProduct != null)
                {                   
                    categoryCB.SelectedItem = category;
                    break;
                }                    
            }
           
            this.edit = edit;

            if (edit)
            {
                Title = "Редактировать";
            }
            else
            {
                Title = "Инфо о товаре";
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

        private void addB_Click(object sender, RoutedEventArgs e)
        {
            BitmapEncoder encoder = null;
            encoder = new PngBitmapEncoder();

            byte[] array = null;

            if (avatarPB != null && avatarPB.Source != null)
            {
                //encoder?.Frames.Add(BitmapFrame.Create(imageUri));
                //MemoryStream imageStream = new MemoryStream();
                //encoder?.Save(imageStream);
                ImageConveter.SendImage(ImageConveter.ImageToByteArray(avatarPB.Source as ImageSource));
            }

            if (add)
            {
                if (weightTB.Text == "")
                    weightTB.Text = "0";
                thisApp.client.Add(new CategoryWithProduct()
                {
                    Name = (categoryCB.SelectedItem as CategoryWithProduct).Name,
                    poductList = new ProductElement[]
                    {
                        new ProductElement()
                        {
                            Descriptions = descriptionsTB.Text,
                            Dimensions = dimensionsTB.Text,
                            //Image = null,
                            Name = nameTB.Text,
                            Weight = Convert.ToDecimal(weightTB.Text),
                            Price = Convert.ToDecimal(priceTB.Text),
                        }
                    }
                });
            }

            if (edit)
            {
                tmpProduct.Descriptions = descriptionsTB.Text;
                tmpProduct.Dimensions = dimensionsTB.Text;
                tmpProduct.Image = array;
                tmpProduct.Name = nameTB.Text;
                tmpProduct.Weight = Convert.ToDecimal(weightTB.Text);
                tmpProduct.Price = Convert.ToDecimal(priceTB.Text.Replace('.', ','));

                thisApp.client.ChangeProduct(tmpProduct, (categoryCB.SelectedItem as CategoryWithProduct).Name);
            }

            thisApp.products = thisApp.client.GetProducts().ToList();
            Close();
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
                try
                {
                    thisTmpImage = avatarPB;
                    format = selectImageOFD.FileName.Split('.').Last();
                    BitmapImage tmp_image = new BitmapImage();
                    imageUri = new Uri(selectImageOFD.FileName);
                    tmp_image.BeginInit();
                    tmp_image.UriSource = imageUri;
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

            //categoryCB.DataContext = thisApp.products;
            categoryCB.DisplayMemberPath = "Name";

            if (add)
            {
                nameTB.IsReadOnly = categoryCB.IsReadOnly = descriptionsTB.IsReadOnly = weightTB.IsReadOnly = dimensionsTB.IsReadOnly = priceTB.IsReadOnly = false;
                addB.Content = "Добавить";
                selectImageB.Visibility = Visibility.Visible;

                var nullCat = thisApp.products.Find(x => x.Name == nullCatName);

                if (nullCat == null)
                    categoryCB.SelectedItem = nullCatName;
                else
                    categoryCB.SelectedItem = nullCat;
            }
            else
            {
                nameTB.Text = tmpProduct.Name;
                dimensionsTB.Text = tmpProduct.Dimensions;
                weightTB.Text = tmpProduct.Weight.ToString();
                descriptionsTB.Text = tmpProduct.Descriptions;
                priceTB.Text = tmpProduct.Price.ToString();

                if (tmpProduct.Image != null && tmpProduct.Image.Length > 0)
                {
                    var bi = new BitmapImage();
                    bi.BeginInit();
                    //ms = new MemoryStream(thisApp.user.Image);
                    bi.StreamSource = new MemoryStream(tmpProduct.Image);
                    bi.EndInit();
                    avatarPB.Source = bi;
                }

                if (edit)
                {
                    nameTB.IsReadOnly = categoryCB.IsReadOnly = descriptionsTB.IsReadOnly = weightTB.IsReadOnly = dimensionsTB.IsReadOnly = priceTB.IsReadOnly = false;
                    addB.Content = "Сохранить";
                    selectImageB.Visibility = Visibility.Visible;
                }
                else
                {
                    nameTB.IsReadOnly = categoryCB.IsReadOnly = descriptionsTB.IsReadOnly = weightTB.IsReadOnly = dimensionsTB.IsReadOnly = priceTB.IsReadOnly = true;
                    categoryCB.IsEditable = false;
                    addB.Content = "ОК";
                    selectImageB.Visibility = Visibility.Collapsed;
                }
            }           
        }

        private void cancelB_Click(object sender, RoutedEventArgs e)
        {
            //Adminka adm = new Adminka();
            //adm.Show();
            avatarPB = thisTmpImage;
            Close();
        }
    }
}
