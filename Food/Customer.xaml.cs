﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using Xceed.Wpf.DataGrid.Converters;
using System.Drawing;

namespace Food
{
    /// <summary>
    /// Interaction logic for Customer.xaml
    /// </summary>
    public partial class Customer : Window
    {
        App thisApp;
        Service1Client _client;

        int nPassw = 4;
        MemoryStream imageStream;
        private string format;
        private Uri imageUri;
        Image thisTmpImage;

        UserInfo user;

        bool edit;

        public Customer(UserInfo user)
        {
            InitializeComponent();

            Left = (SystemParameters.PrimaryScreenWidth - Width) / 2;
            Top = (SystemParameters.PrimaryScreenHeight - Height) / 2;

            thisApp = Application.Current as App;
            _client = thisApp.client;
            user = thisApp.user;

            edit = true;
        }

        public Customer(int id = 0)
        {
            InitializeComponent();

            Left = (SystemParameters.PrimaryScreenWidth - Width) / 2;
            Top = (SystemParameters.PrimaryScreenHeight - Height) / 2;

            thisApp = Application.Current as App;
            _client = thisApp.client;

            if(id==0)
                user = new UserInfo();
            else
                user = _client.GetUserInfoId(id);

            edit = false;
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

            if (edit)
            {
                //loginTB.IsReadOnly = passwTB.IsReadOnly = emailTB.IsReadOnly = nameTB.IsReadOnly = addressTB.IsReadOnly = false;
               // maleRB.IsEnabled = femaleRB.IsEnabled = true;
                //changeB.Visibility = cancelB.Visibility = Visibility.Collapsed;
                //okB.Visibility = Visibility.Visible;

                loginTB.Text = user.Login;
                passwTB.Text = user.Password;
                emailTB.Text = user.Email;
                nameTB.Text = user.FullName;
                addressTB.Text = user.Address;

                maleRB.IsChecked = user.Sex;
                femaleRB.IsChecked = !user.Sex;

                Title = "Кабинет";
            }
            else
            {
                Title = "Регистрация";
                changeB.Content = "Зарегистрироваться";
                femaleRB.IsChecked = !(maleRB.IsChecked = true);
            }            

            if (user.Image != null && user.Image.Length > 0)
            {
                var bi = new BitmapImage();
                bi.BeginInit();
                //ms = new MemoryStream(thisApp.user.Image);
                bi.StreamSource = new MemoryStream(user.Image);
                bi.EndInit();
                avatarPB.Source = bi;
            }
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
                    drkMB.Show();
                    avatarPB = thisTmpImage;
                }
            }
        }
        private void changeB_Click(object sender, RoutedEventArgs e)
        {
            if (loginTB.Text == "")
            {
                DarkMessageBox dmb = new DarkMessageBox("Не указан логин", "Ошибка");
                dmb.ShowDialog();
                return;
            }
            if (passwTB.Text == "")
            {
                DarkMessageBox dmb = new DarkMessageBox("Не указан пароль", "Ошибка");
                dmb.ShowDialog();
                return;
            }
            if (emailTB.Text == "")
            {
                DarkMessageBox dmb = new DarkMessageBox("Не указан email", "Ошибка");
                dmb.ShowDialog();
                return;
            }
            
            //Проверка корректности email----------------- 
            string pattern =
                @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$";

            if (!Regex.IsMatch(emailTB.Text, pattern, RegexOptions.IgnoreCase))
            {
                DarkMessageBox dmb = new DarkMessageBox("Не корректный Email", "Ошибка");
                dmb.ShowDialog();
                return;
            }
            //----------------------------------------    

            if (passwTB.Text.Length < nPassw)
            {
                DarkMessageBox dmb = new DarkMessageBox("Пароль слишком короткий. Укажите пароль не короче " + nPassw + " символов", "Ошибка");
                dmb.ShowDialog();

                return;
            }

            UserInfo tmpUser = null;

            if (edit)
            {
                tmpUser = thisApp.user;
            }
            else
            {
                tmpUser = new UserInfo() {Login = ""};
            }
           
            var oldLogin = tmpUser.Login;
            tmpUser.Login = loginTB.Text;
            tmpUser.Password = passwTB.Text;
            tmpUser.Email = emailTB.Text;
            tmpUser.FullName = nameTB.Text;
            tmpUser.Address = addressTB.Text;

            tmpUser.Sex = maleRB.IsChecked.Value;

            BitmapEncoder encoder = null;
            encoder = new PngBitmapEncoder();

            MemoryStream imageStream = null;

            if (imageUri != null)
            {
                encoder?.Frames.Add(BitmapFrame.Create(imageUri));
            }               
            else
            {
                //ImageSource imgSource = avatarPB.Source.Clone();
                //BitmapImage bitmapImage = imgSource as BitmapImage;

                //imageStream = bitmapImage.StreamSource as MemoryStream;
                Uri uri = new Uri("pack://application:,,,/Food;component/images/noUserImage.jpg");
                encoder?.Frames.Add(BitmapFrame.Create(uri));
            }
            imageStream = new MemoryStream();
            encoder?.Save(imageStream);

            tmpUser.Image = imageStream.ToArray();

            BinaryWriter

            _client.SetUserInfo(oldLogin, tmpUser);

            Close();
        }

        private void cancelB_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
