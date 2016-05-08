using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Server
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        static int imagePartSize = 32768; //размер куска изображения для передачи
        static List<byte[]> imageList = new List<byte[]>();
        static byte[] imageArray;

        public string Add(CategoryWithProduct category) //добавление продукта и/или категории
        {
            DataTable categoryTable = DataBase.LoadTableFromDataBase("category");
            DataTable prodCatTable = DataBase.LoadTableFromDataBase("productCategory");
            DataTable productTable = DataBase.LoadTableFromDataBase("productTable");

            DataRow[] tmpRows = null;
            DataRow currCategory = null;
            DataRow row = null;

            string rezult = "Нет данных для добавления";

            if (!string.IsNullOrEmpty(category.Name))
            {
                tmpRows = DataBase.SelectInTable(categoryTable, $"name = '{category.Name}'"); //искать в базе категорию
                if (tmpRows.Length == 0) //если нету, создать новую
                {
                    row = categoryTable.NewRow();
                    row["name"] = category.Name;
                    categoryTable.Rows.Add(row);
                    DataBase.UpdateTableIntoDataBase(categoryTable);
                    rezult = "Новая категория добавлена";
                }

                if (category.poductList == null || category.poductList.Count <= 0) //
                    return rezult;

                currCategory = tmpRows[0];
                row = productTable.NewRow();
                row["name"] = category.poductList[0].Name;
                row["weight"] = category.poductList[0].Weight;
                row["dimensions"] = category.poductList[0].Weight;
                row["description"] = category.poductList[0].Descriptions;
                //if(category.poductList[0].Image != null && category.poductList[0].Image.Length > 0)\
                //  row["photo"] = category.poductList[0].Image;
                if (imageArray != null && imageArray.Length > 0)
                    row["photo"] = imageArray;                    
                row["price"] = category.poductList[0].Price;

                productTable.Rows.Add(row);

                DataBase.UpdateTableIntoDataBase(productTable);
                productTable = DataBase.LoadTableFromDataBase("productTable");

                DataRow currProduct = productTable.Rows[productTable.Rows.Count - 1];

                row = prodCatTable.NewRow();
                row["idProduct"] = currProduct["idProduct"];
                row["nameCategory"] = currCategory["name"];
                prodCatTable.Rows.Add(row);
                DataBase.UpdateTableIntoDataBase(prodCatTable);
                rezult = "Добавление успешно";
            }
            return rezult;                
        }
        public string Delete(CategoryWithProduct category, bool delProduct)// удаление категории и/или продукта
        {
            var categoryTable = DataBase.LoadTableFromDataBase("category");
            var prodCatTable = DataBase.LoadTableFromDataBase("productCategory");
            var productTable = DataBase.LoadTableFromDataBase("productTable");

            string rezult = "Не удалено";

            DataRow[] tmpRows = null;
            /////////////////Удаление категории////////////////////////////            
            if (category.Name != null)
            {
                tmpRows = DataBase.SelectInTable(categoryTable, $"name = '{category.Name}'"); //получение категории

                if (tmpRows.Length == 1)
                {
                    var prodCatRows = DataBase.SelectInTable(prodCatTable, $"nameCategory = '{category.Name}'"); //получение товаров категории

                    foreach (var prodCatRow in prodCatRows)
                    {
                        if (delProduct)
                        {
                            var prodRow = DataBase.SelectInTable(productTable,
                                $"idProduct = '{prodCatRow["idProduct"]}'");
                            if (prodRow.Length == 1)
                            {
                                prodRow[0].Delete();
                            }
                            prodCatRow.Delete();
                        }
                        else
                        {
                            prodCatRow["nameCategory"] = "Без Категории";
                        }
                    }

                    if (tmpRows[0]["name"].ToString() != "Без Категории")
                    {
                        tmpRows[0].Delete(); //удаление категории в таблице категорий
                    }
                        

                    DataBase.UpdateTableIntoDataBase(categoryTable);
                    DataBase.UpdateTableIntoDataBase(prodCatTable);
                    DataBase.UpdateTableIntoDataBase(productTable);
                    rezult = "Успешно удалено";
                }
            }           
            //------------------------Удаление продукта(-ов)--------------------
            if (category.poductList != null)//удаление продукта(-ов)
            {
                /////////////////////////////////////////
                foreach (var prod in category.poductList)
                {
                    tmpRows = DataBase.SelectInTable(prodCatTable, $"idProduct = '{prod.Id}'");
                    if (tmpRows.Length == 1)
                    {
                        tmpRows[0].Delete();
                    }

                    tmpRows = DataBase.SelectInTable(productTable, $"idProduct = {prod.Id}");
                    if (tmpRows.Length == 1)
                    {
                        tmpRows[0].Delete();
                    }
                }
                DataBase.UpdateTableIntoDataBase(prodCatTable);
                DataBase.UpdateTableIntoDataBase(productTable);
                rezult = "Продект удалён";
            }

            return rezult;
        }
                              
        public List<CategoryWithProduct> GetProducts()//получение категорий с товарами
        {
            DataTable prodCatTable = DataBase.LoadTableFromDataBase("productCategory");
            DataTable categoryTable = DataBase.LoadTableFromDataBase("category");
            DataTable productTable = DataBase.LoadTableFromDataBase("productTable");

            if (prodCatTable == null || categoryTable == null || productTable == null)
                return null;

            var listCategory = new List<CategoryWithProduct>();

            foreach (DataRow row in categoryTable.Rows)
            {
                var tmpCategory = new CategoryWithProduct();
                tmpCategory.Name = row["name"].ToString();
                tmpCategory.Id = (int)row["IdCategory"];

                string str = String.Format("nameCategory = '{0}'", row["name"].ToString());

                DataRow[] tmpProdRows1 = DataBase.SelectInTable(prodCatTable, str);
                if (tmpProdRows1.Length == 0) { listCategory.Add(tmpCategory); continue;}
                tmpCategory.poductList = new List<ProductElement>();
                foreach (DataRow tmpRow in tmpProdRows1)
                {
                    DataRow[] tmpProdRows2 = DataBase.SelectInTable(productTable, $"idProduct = '{tmpRow["idProduct"]}'");
                    if(tmpProdRows2.Length == 0) continue;
                    var tmpProduct = new ProductElement();

                    tmpProduct.Id = (int) tmpProdRows2[0]["idProduct"];
                    tmpProduct.Name = tmpProdRows2[0]["name"].ToString();
                    tmpProduct.Image = tmpProdRows2[0]["photo"] as byte[];
                    tmpProduct.Descriptions = tmpProdRows2[0]["description"].ToString();
                    tmpProduct.Dimensions = tmpProdRows2[0]["dimensions"].ToString();
                    tmpProduct.Weight = Convert.ToDecimal(tmpProdRows2[0]["weight"]);
                    tmpProduct.Price = (decimal) tmpProdRows2[0]["price"];
                    tmpProduct.Visibility = false;
                    
                    tmpCategory.poductList.Add(tmpProduct);
                }

                listCategory.Add(tmpCategory);
            }

            return listCategory;
        }
        public string ChangeProduct(ProductElement product, string newCategory)//перемещение товара в другую категорию
        {
            var prodCatTable = DataBase.LoadTableFromDataBase("productCategory");
            var productTable = DataBase.LoadTableFromDataBase("productTable");

            var tmpRows = DataBase.SelectInTable(prodCatTable, $"idProduct = '{product.Id}'");
            if (tmpRows.Length == 1)
            {
                tmpRows[0]["nameCategory"] = newCategory;
            }
            DataBase.UpdateTableIntoDataBase(prodCatTable);

            tmpRows = DataBase.SelectInTable(productTable, $"idProduct = '{product.Id}'");
            if (tmpRows.Length == 1)
            {
                tmpRows[0]["name"] = product.Name;
                tmpRows[0]["weight"] = product.Weight;
                tmpRows[0]["dimensions"] = product.Dimensions;
                tmpRows[0]["description"] = product.Descriptions;
                //if(product.Image != null && product.Image.Length > 0)
                //    tmpRows[0]["photo"] = product.Image;
                if (imageArray != null && imageArray.Length > 0)
                    tmpRows[0]["photo"] = imageArray;
                tmpRows[0]["price"] = product.Price;
            }
            DataBase.UpdateTableIntoDataBase(productTable);
            return "Изменение успешно";
        }

        public UserInfo GetUserInfo(string login, string password, string rank)
        {
            DataTable customerTable = DataBase.LoadTableFromDataBase("customerTable");

            DataRow[] tmpDataRow = new DataRow[0];

            var uif = new UserInfo();
            if (rank == "Покупатель")
            {
                tmpDataRow = DataBase.SelectInTable(customerTable,
                    $"login = '{login}' AND password = '{password}'");

                if (tmpDataRow.Length == 0)
                {
                    uif.Message = "Покупатель с таким Логином и/или Паролем не найден";
                    return uif;
                }
            }
            else if (rank == "Менеджер")
            {
                tmpDataRow = DataBase.SelectInTable(customerTable,
                    $"login = '{login}' AND password = '{password}' AND rank = '{rank}' OR rank = 'Администратор'");

                if (tmpDataRow.Length == 0)
                {
                    uif.Message = "Менеджер с таким Логином и/или Паролем не найден";
                    return uif;
                }
            }
            else if (rank == "Администратор")
            { 
                tmpDataRow = DataBase.SelectInTable(customerTable,
                    $"login = '{login}' AND password = '{password}' AND rank = '{rank}'");

                if (tmpDataRow.Length == 0)
                {
                    uif.Message = "Администратор с таким Логином и/или Паролем не найден";
                    return uif;
                }
            }

            uif.Id = (int) tmpDataRow[0]["idCustomer"];
            uif.Login = tmpDataRow[0]["login"].ToString();
            uif.Password = tmpDataRow[0]["password"].ToString();
            uif.Address = tmpDataRow[0]["address"].ToString();
            uif.Email = tmpDataRow[0]["email"].ToString();
            uif.FullName = tmpDataRow[0]["full_name"].ToString();
            uif.Rank = tmpDataRow[0]["rank"].ToString();
            uif.Sex = Convert.ToBoolean(tmpDataRow[0]["sex"]);
            uif.Image = tmpDataRow[0]["photo"] as byte[];
            uif.Message = "OK";

            return uif;
        }

        public UserInfo GetUserInfoId(int Id)
        {
            DataTable customerTable = DataBase.LoadTableFromDataBase("customerTable");

            DataRow[] tmpDataRow = new DataRow[0];

            var uif = new UserInfo();

            tmpDataRow = DataBase.SelectInTable(customerTable, $"idCustomer = {Id}");

            if (tmpDataRow.Length == 0)
            {
                uif.Message = "Покупатель с таким Логином и/или Паролем не найден";
                return uif;
            }

            uif.Id = (int)tmpDataRow[0]["idCustomer"];
            uif.Login = tmpDataRow[0]["login"].ToString();
            uif.Password = tmpDataRow[0]["password"].ToString();
            uif.Address = tmpDataRow[0]["address"].ToString();
            uif.Email = tmpDataRow[0]["email"].ToString();
            uif.FullName = tmpDataRow[0]["full_name"].ToString();
            uif.Rank = tmpDataRow[0]["rank"].ToString();
            uif.Sex = Convert.ToBoolean(tmpDataRow[0]["sex"]);
            uif.Image = tmpDataRow[0]["photo"] as byte[];
            uif.Message = "OK";

            return uif;
        }
        public string SetUserInfo(string login, UserInfo user)//изменение данных пользователя
        {
            DataTable customerTable = DataBase.LoadTableFromDataBase("customerTable");

            DataRow [] tmpDataRow = DataBase.SelectInTable(customerTable, $"login = '{login}'");

            string rezult = "Нет данных для добавления";

            if (tmpDataRow.Length == 0) //добавление пользователя
            {
                for (int i = 0; i < customerTable.Rows.Count; i++)
                {
                    if ((string)customerTable.Rows[i]["login"] == user.Login)
                        return "Покупатель с таким логином уже существует";
                }

                DataRow row = customerTable.NewRow();

                row["login"] = user.Login;
                row["password"] = user.Password;
                row["email"] = user.Email;
                row["full_name"] = user.FullName;
                row["address"] = user.Address;
                //row["photo"] = user.Image;
                if (imageArray != null && imageArray.Length > 0)
                    row["photo"] =imageArray;
                row["sex"] = user.Sex;

                customerTable.Rows.Add(row);

                rezult = "Новый покупатель зарегистрирован";
            }
            else
            {
                tmpDataRow[0]["login"] = user.Login;
                tmpDataRow[0]["password"] = user.Password;
                tmpDataRow[0]["email"] = user.Email;
                tmpDataRow[0]["full_name"] = user.FullName;
                tmpDataRow[0]["address"] = user.Address;
                //if(user.Image != null && user.Image.Length > 0)
                //    tmpDataRow[0]["photo"] = user.Image;
                if (imageArray != null && imageArray.Length > 0)
                    tmpDataRow[0]["photo"] = imageArray;
                tmpDataRow[0]["sex"] = user.Sex;

                rezult = "Данные успешно обновлены";
            }            

            DataBase.UpdateTableIntoDataBase(customerTable);

            return rezult;
        }

        public Order[] GetOrders(int userId = 0)
        {
            string str = typeof(Stream).ToString();
            DataTable orderTable = DataBase.LoadTableFromDataBase("orderTable");
            DataTable orderProduct = DataBase.LoadTableFromDataBase("orderProduct");
            DataTable productTable = DataBase.LoadTableFromDataBase("productTable");

            List<Order> tmpList = new List<Order>();

            DataRow [] orderRows = null;

            if (userId > 0)
            {
                orderRows = DataBase.SelectInTable(orderTable, $"idCustomer = {userId}");
            }
            else
            {
                orderRows = DataBase.SelectInTable(orderTable, "idOrder > 0");
            }

            foreach (DataRow row in orderRows)
            {
                var products = new List<ProductElement>();

                var tmpRows = DataBase.SelectInTable(orderProduct, $"idOrder = {row["idOrder"]}");

                foreach (DataRow idProduct in tmpRows)
                {
                    var tmpProd = DataBase.SelectInTable(productTable, $"idProduct = {idProduct["idProduct"]}")[0];

                    products.Add(new ProductElement()
                    {
                       Id = (int) tmpProd["idProduct"],
                       Name = tmpProd["name"].ToString(),
                       Weight = Convert.ToDecimal(tmpProd["weight"]),
                       Image = (byte[])tmpProd["photo"],
                       Descriptions = tmpProd["description"].ToString(),
                       Dimensions = tmpProd["dimensions"].ToString(),
                       Price = (decimal)tmpProd["price"],
                    });
                }
                tmpList.Add(new Order()
                {
                    Id = (int) row["idOrder"],
                    IdCustomer = (int) row["idCustomer"],
                    products = products.ToArray(),
                    Price = Convert.ToDecimal(row["price"]),
                    Time = (DateTime) row["dateAdd"],
                    CardNumber = row["cardNumber"].ToString(),
                    CardCVV2 = row["cvv2Number"].ToString(),
                    CardDate = row["cardDate"].ToString()
                });
            }

            return tmpList.ToArray();
        }

        public string AddOrder(Order order)
        {
            DataTable orderTable = DataBase.LoadTableFromDataBase("orderTable");
            DataTable orderProduct = DataBase.LoadTableFromDataBase("orderProduct");

            var orderRow = orderTable.NewRow();
            orderRow["idCustomer"] = order.IdCustomer;
            orderRow["dateAdd"] = order.Time;
            orderRow["price"] = order.Price;
            orderRow["cardNumber"] = order.CardNumber;
            orderRow["cvv2Number"] = order.CardCVV2;
            orderRow["cardDate"] = order.CardDate;

            orderTable.Rows.Add(orderRow);
            DataBase.UpdateTableIntoDataBase(orderTable);
            orderTable = DataBase.LoadTableFromDataBase("orderTable");
            var lasrOrder = orderTable.Rows[orderTable.Rows.Count - 1];


            foreach (var item in order.products)
            {
                var opRow = orderProduct.NewRow();
                opRow["idOrder"] = lasrOrder["idOrder"];
                opRow["idProduct"] = item.Id;
                orderProduct.Rows.Add(opRow);
            }

            DataBase.UpdateTableIntoDataBase(orderProduct);

            return "ОК";
        }

        public void RecieveImage(byte[] array_part, int i, int nParts)
        {
            if (i <= 0)
            {
                imageList.Clear();
                imageArray = null;
            }
                

            imageList.Add(array_part);

            if (i >= nParts - 1)
            {
                imageArray = new byte[(imageList.Count - 1) * imagePartSize + array_part.Length];
                for (int j = 0; j < imageList.Count - 1; j++)
                {
                    Array.Copy(imageList[j], 0, imageArray, j*imagePartSize, imagePartSize);
                }
                Array.Copy(imageList.Last(), 0, imageArray, (imageList.Count-1)*imagePartSize, array_part.Length);
            }
        }

        public int GetPartSize()
        {
            return imagePartSize;
        }
    }
}
