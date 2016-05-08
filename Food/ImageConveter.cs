using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Food
{
    public class ImageConveter
    {
        public static byte [] ImageToByteArray(ImageSource image)
        {
            if (image == null) return new byte[0];

            BitmapEncoder encoder = null;
            encoder = new PngBitmapEncoder();

            encoder?.Frames.Add(BitmapFrame.Create(image as BitmapSource));                 

            MemoryStream imageStream = new MemoryStream();
            encoder?.Save(imageStream);

            return imageStream.ToArray();
        }
        public static BitmapImage ByteArrayToImage(byte [] array)
        {
            if(array == null || array.Length <= 0)
                return null;

            var stream = new MemoryStream(array);
            var bmImage = new BitmapImage();
            bmImage.BeginInit();
            bmImage.StreamSource = stream;
            bmImage.EndInit();

            return bmImage;
        }

        public static void SendImage(byte[] array)
        {
            App thisApp = Application.Current as App;
            int size = thisApp.client.GetPartSize();
            int nParts;

            if (array.Length % size > 0)
            {
                nParts = (int) (array.Length / size) + 1;

                var tmpArray = new byte[0];
                for (int i = 0; i < nParts - 1; i++)
                {
                    tmpArray = new byte[size];
                    Array.Copy(array, i * size, tmpArray, 0, size);
                    thisApp.client.RecieveImage(tmpArray, i, nParts);
                }

                int lastSize = array.Length - size*(nParts - 1);

                tmpArray = new byte[lastSize];
                Array.Copy(array, (nParts-1) * size, tmpArray, 0, lastSize);
                thisApp.client.RecieveImage(tmpArray, nParts - 1, nParts);
            }
            else
            {
                nParts = (int) (array.Length/size);

                var tmpArray = new byte[0];
                for (int i = 0; i < nParts; i++)
                {
                    tmpArray = new byte[size];
                    Array.Copy(array, i * size, tmpArray, 0, size);
                    thisApp.client.RecieveImage(tmpArray, i, nParts);
                }
            }
        }
    }
}
