using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Server
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        public void Add(CategoryWithProduct category) //добавление продукта и/или категории
        {
            DataTable categoryTable = DataBase.LoadTableFromDataBase("category");
            DataTable prodCatTable = DataBase.LoadTableFromDataBase("productCategory");
            DataTable productTable = DataBase.LoadTableFromDataBase("productTable");

            DataRow[] tmpRows = null;
            DataRow currCategory = null;
            DataRow row = null;

            if (!string.IsNullOrEmpty(category.Name))
            {
                tmpRows = DataBase.SelectInTable(categoryTable, $"name = '{category.Name}'"); //искать в базе категорию
                if (tmpRows.Length == 0) //если нету, создать новую
                {
                    row = categoryTable.NewRow();
                    row["name"] = category.Name;
                    categoryTable.Rows.Add(row);
                    DataBase.UpdateTableIntoDataBase(categoryTable);                  
                }

                if (category.poductList.Count <= 0) //
                    return;

                currCategory = tmpRows[0];
                row = productTable.NewRow();
                row["name"] = category.poductList[0].Name;
                row["weight"] = category.poductList[0].Weight;
                row["dimensions"] = category.poductList[0].Weight;
                row["description"] = category.poductList[0].Descriptions;
                row["photo"] = category.poductList[0].Image;
                row["price"] = category.poductList[0].Price;

                productTable.Rows.Add(row);

                DataBase.UpdateTableIntoDataBase(productTable);
                DataRow currProduct = row;

                row = prodCatTable.NewRow();
                row["idProduct"] = currProduct["idProduct"];
                row["nameCategory"] = currCategory["name"];
                prodCatTable.Rows.Add(row);
                DataBase.UpdateTableIntoDataBase(prodCatTable);
            }                
        }
        public void Delete(CategoryWithProduct category, bool delProduct)// удаление категории и/или продукта
        {
            var categoryTable = DataBase.LoadTableFromDataBase("category");
            var prodCatTable = DataBase.LoadTableFromDataBase("productCategory");
            var productTable = DataBase.LoadTableFromDataBase("productTable");

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

                    tmpRows[0].Delete(); //удаление категории в таблице категорий

                    DataBase.UpdateTableIntoDataBase(categoryTable);
                    DataBase.UpdateTableIntoDataBase(prodCatTable);
                    DataBase.UpdateTableIntoDataBase(productTable);
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
            }          
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
        public void ChangeProduct(ProductElement product, string newCategory)//перемещение товара в другую категорию
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
                tmpRows[0]["photo"] = product.Image;
                tmpRows[0]["price"] = product.Price;
            }
            DataBase.UpdateTableIntoDataBase(productTable);
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
        public void SetUserInfo(string login, UserInfo user)//изменение данных пользователя
        {
            DataTable customerTable = DataBase.LoadTableFromDataBase("customerTable");

            DataRow [] tmpDataRow = DataBase.SelectInTable(customerTable, $"login = '{login}'");

            if (tmpDataRow.Length == 0) //добавление пользователя
            {
                DataRow row = customerTable.NewRow();

                row["login"] = user.Login;
                row["password"] = user.Password;
                row["email"] = user.Email;
                row["full_name"] = user.FullName;
                row["address"] = user.Address;
                row["photo"] = user.Image;
                row["sex"] = user.Sex;

                customerTable.Rows.Add(row);
            }
            else
            {
                tmpDataRow[0]["login"] = user.Login;
                tmpDataRow[0]["password"] = user.Password;
                tmpDataRow[0]["email"] = user.Email;
                tmpDataRow[0]["full_name"] = user.FullName;
                tmpDataRow[0]["address"] = user.Address;
                tmpDataRow[0]["photo"] = user.Image;
                tmpDataRow[0]["sex"] = user.Sex;
            }            

            DataBase.UpdateTableIntoDataBase(customerTable);
        }

        public List<Order> GetOrders()
        {
            DataTable orderTable = DataBase.LoadTableFromDataBase("orderTable");
            DataTable orderProduct = DataBase.LoadTableFromDataBase("orderProduct");

            List<Order> tmpList = new List<Order>();

            foreach (DataRow row in orderTable.Rows)
            {
                var products = new List<ProductElement>();

                var tmpRows = DataBase.SelectInTable(orderProduct, $"idOrder = {row["idOrder"]}");

                foreach (DataRow product in tmpRows)
                {
                    products.Add(new ProductElement()
                    {
                       Id = (int) product["idProduct"],
                       Name = product["name"].ToString(),
                       Weight = (decimal) product["weight"],
                       Image = (byte[]) product["photo"],
                       Descriptions = product["description"].ToString(),
                       Dimensions = product["dimensions"].ToString(),
                       Price = (decimal) product["price"],
                    });
                }
                tmpList.Add(new Order()
                {
                    Id = (int) row["idOrder"],
                    IdCustomer = (int) row["idCustomer"],
                    products = products.ToArray(),
                    Price = (decimal) row["price"],
                    Time = (DateTime) row["dateAdd"],
                    CardNumber = row["cardNumber"].ToString(),
                    CardCVV2 = row["cvv2Number"].ToString(),
                    CardDate = row["cardDate"].ToString()
                });
            }

            return tmpList;
        }

        public void AddOrder(Order order)
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

            foreach (var item in order.products)
            {
                var opRow = orderProduct.NewRow();
                opRow["idOrder"] = orderRow["idOrder"];
                opRow["idProduct"] = item.Id;
                opRow["count"] = item.Count;
                orderProduct.Rows.Add(opRow);
            }

            DataBase.UpdateTableIntoDataBase(orderProduct);
        }
    }
}
