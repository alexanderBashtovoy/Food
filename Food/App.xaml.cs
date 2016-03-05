using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using Food.ServiceReference1;

namespace Food
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public UserInfo user { get; set; }
        public List<CategoryWithProduct> products;
        public readonly Service1Client client;
        public string NewCategory;
        public string lastDialogResult;
        public enum Window
        {
            NULL, AddCategory, AddProduct, Admin, Buy, Customer, EditProduct, Manager,
            Registration, Shop, Exit
        };
        public Window NextWindow { get; set; }
        public Window PrevWindow { get; set; }
        public App()
        {
            //StartupUri = new System.Uri("Autorization.xaml", System.UriKind.Relative);
            //var tilingThread = new Thread(new ParameterizedThreadStart(Tiling.ProcessTiling));

            //tilingThread.Start(this);
            try
            {
                client = new Service1Client();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message, "");
            }
            
        }

        public void EnableCheck(bool enable)
        {
            if(products != null)
            foreach (var category in products)
            {
                if (category.poductList != null)
                {
                    foreach (var product in category.poductList)
                    {
                        product.Visibility = enable;
                    }
                }
            }
        }

        public static class Tiling
        {
            public static int id;
            public enum Window
            {
                NULL, AddCategory, AddProduct, Admin, Buy, Customer, EditProduct, Manager,
                Registration, Shop, Exit
            };

            public static Window NextWindow { get; set; }
            public static Window PrevWindow { get; set; }
        }
    }
}
