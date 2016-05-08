using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
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
                //var bind = new BasicHttpBinding();
                //bind.Name = "BasicHttpBindingConf";
                //bind.MaxReceivedMessageSize = Int32.MaxValue;
                //bind.MaxBufferPoolSize = Int32.MaxValue;
                //bind.MaxBufferSize = Int32.MaxValue;
                //bind.ReaderQuotas.MaxArrayLength = Int32.MaxValue;
                //bind.ReaderQuotas.MaxStringContentLength = Int32.MaxValue;
                //bind.CloseTimeout = new TimeSpan(23, 59, 59);
                //bind.OpenTimeout = new TimeSpan(23, 59, 59);
                //bind.SendTimeout = new TimeSpan(23, 59, 59);
                //bind.ReceiveTimeout = new TimeSpan(23, 59, 59);

                //var endPoint = new EndpointAddress("http://localhost:21920/Service1.svc");
                

                //client = new Service1Client(bind, endPoint);


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
