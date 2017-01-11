using System.Collections.Generic;

namespace Cloth3D.Data {
    public class OrderInfo {
        public string OrderCode = string.Empty;
        public string OrderAmount = string.Empty;
        public string OrderStatus = string.Empty;
        public string OrderMemo = string.Empty;
        public string OrderId = string.Empty;
        public string ClothNumber = string.Empty;
        public string Store_id = string.Empty;
        public string Store_name = string.Empty;
        public string Order_address = string.Empty;
        public string Order_tel = string.Empty;
        public string Order_contact = string.Empty;
        public string Order_time = string.Empty;
        public string OrderPrice = string.Empty;
        public string OrderImage = string.Empty;
        public List<Goods> Order_Goods = new List<Goods>();
    }
}