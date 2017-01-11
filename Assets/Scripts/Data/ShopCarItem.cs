
using System.Runtime.Remoting.Messaging;
using LitJson;

namespace Cloth3D.Data {
    public class ShopCarItem {
        public string ModelId = string.Empty;
        public string Id = string.Empty;
        public string GoodsId = string.Empty;
        public int GoodsNum = 0;
        public string GoodsPrice = string.Empty;
        public string GoodsPic = string.Empty;//图片
        public string GoodsName = string.Empty;
    }
}
