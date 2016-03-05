using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Server
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {        
        [OperationContract]
        List<CategoryWithProduct> GetProducts();
        // TODO: Add your service operations here
        [OperationContract]
        UserInfo GetUserInfo(string login, string password, string rank);
        [OperationContract]
        UserInfo GetUserInfoId(int Id);
        [OperationContract]
        void SetUserInfo(string login, UserInfo user);
        [OperationContract]
        void Delete(CategoryWithProduct category, bool delProduct);
        [OperationContract]
        void Add(CategoryWithProduct category);
        [OperationContract]
        void ChangeProduct(ProductElement product, string newCategory);
        [OperationContract]
        List<Order> GetOrders();
        [OperationContract]
        void AddOrder(Order order);
    }
    [DataContract]
    public class ProductElement
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public byte[] Image { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Dimensions { get; set; }
        [DataMember]
        public decimal Weight { get; set; }
        [DataMember]
        public string Descriptions { get; set; }
        [DataMember]
        public bool IsSelected { get; set; }
        [DataMember]
        public bool Visibility { get; set; }
        [DataMember]
        public decimal Price { get; set; }
        [DataMember]
        public int Count { get; set; }
    }
    [DataContract]
    public class CategoryWithProduct
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public List<ProductElement> poductList { get; set; }
        [OperationContract]
        public override string ToString()
        {
            return Name;
        }
    }
    [DataContract]
    public class UserInfo
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Login { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string FullName { get; set; }
        [DataMember]
        public string Address { get; set; }
        [DataMember]
        public bool Sex { get; set; }
        [DataMember]
        public byte[] Image { get; set; }
        [DataMember]
        public string Rank { get; set; }
        [DataMember]
        public string Message { get; set; }
    }
    [DataContract]
    public class Order
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int IdCustomer { get; set; }
        [DataMember]
        public ProductElement[] products { get; set; }
        [DataMember]
        public DateTime Time { get; set; }
        [DataMember]
        public decimal Price { get; set; }
        [DataMember]
        public string CardNumber { get; set; }
        [DataMember]
        public string CardDate { get; set; }
        [DataMember]
        public string CardCVV2 { get; set; }
    }
}
